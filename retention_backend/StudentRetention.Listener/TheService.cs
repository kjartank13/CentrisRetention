using System;
using System.Threading;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using RabbitMQ.Client;
using Nest;
using System.Linq;
using Newtonsoft.Json.Linq;
using StudentRetentionAPI.Services.Services;
using StudentRetentionAPI.Services.Repositories;

namespace StudentRetention.Listener
{
	internal partial class TheService : ServiceBase
	{
		private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
		private readonly StudentService _studentService;
		private readonly MessageService _messageService;
		private readonly JArray _dailyList = new JArray();
		private Thread _rabbittThread;
		private Thread _dailyThread;
		private Timer _timer;
		private string _elasticUriString;
		private string _elasticIndexName;
		private string _rabbitQueueName;
		private string _rabbitexchangeName;
		private string _rabbitEncoding;
		private string _exchangeType;

		/// <summary>
		/// this main function checks if we are running service through Visual Studio(design) or as Microsoft Service
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			TheService service = new TheService();
			if (Environment.UserInteractive)
			{
				// For debugging (since services won't function properly as apps (they throw errors)
				service.OnStart(args);
				Console.ReadLine();
				// service.OnStop();
			}
			else
			{
				Run(service);
			}
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public TheService()
		{
			IUnitOfWork uow = new UnitOfWork<AppDataContext>();
			_studentService = new StudentService(uow);
			_messageService = new MessageService(uow);
			InitializeComponent();
		}

		/// <summary>
		/// This is the main function that starts the whole service and sets timer on other services.
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
		{
			SetUpTimer(new TimeSpan(04, 00, 00));
			// Setup the Thread for rabbit messagequeue
			_rabbittThread = new Thread(RabbitThreadFunc)
			{
				Name = "Rabbit",
				IsBackground = true
			};
			// Setup the Thread for elastic Database
			_rabbittThread.Start();
		}

		/// <summary>
		/// This class deconstructor is to kill threads when they have finished ... only in userinteractive Enveironment.
		/// </summary>
		protected override void OnStop()
		{
			_shutdownEvent.Set();
			// give the thread 3 seconds to stop
			try
			{
				if (!_rabbittThread.Join(3000))
				{
					_rabbittThread.Abort();
				}
			}
			catch (ThreadStateException e)
			{
				Console.WriteLine("Caught: {0}", e.Message);
			}
			// give the thread 3 seconds to stop
			try
			{
				if (!_dailyThread.Join(3000))
				{
					_dailyThread.Abort();
				}
			}
			catch (ThreadStateException e)
			{
				Console.WriteLine("Caught: {0}", e.Message);
			}
		}

		/// <summary>
		/// This function will be executed by RabbitThread and listens for all incoming events from the messagequeu
		/// It sends all messages that arrive to Elastic database
		/// </summary>
		private void RabbitThreadFunc()
		{
			while (!_shutdownEvent.WaitOne(0))
			{
				#region Fetch connection parameters from app.config needed to connect to Messagequeue

				ConnectionFactory factory = new ConnectionFactory()
				{
					HostName = ConfigurationManager.AppSettings["hostName"],
					UserName = ConfigurationManager.AppSettings["userName"],
					Password = ConfigurationManager.AppSettings["password"]
				};

				#endregion

				// Set up connection and listen to the queue for events 
				// when event arrives parse to json object and send to Elastic search
				using (var connection = factory.CreateConnection())
				{
					using (var channel = connection.CreateModel())
					{
						_rabbitexchangeName = ConfigurationManager.AppSettings.Get("RabbitExchangeName");
						_rabbitQueueName    = ConfigurationManager.AppSettings.Get("RabbitQueueName");
						_rabbitEncoding     = ConfigurationManager.AppSettings.Get("RabbitEncoding");
						_elasticIndexName   = ConfigurationManager.AppSettings.Get("ElasticIndexName");
						_elasticUriString   = ConfigurationManager.AppSettings.Get("ElasticUriString");
						_exchangeType       = ConfigurationManager.AppSettings.Get("ExchangeType");
						channel.QueueDeclare(_rabbitQueueName, true, false, false, null);
						channel.ExchangeDeclare(_rabbitexchangeName, _exchangeType);

						// bindingKey declares what events we want from rabbitMQ 
						// "#" is a wild for all types
						// "*" is a wild for specific type
						var bindingKey = "#";
						//bind our connection to specific queue from a "rabbitQueueName" our case "Centris" pool and all events at this moment "#"
						channel.QueueBind(_rabbitQueueName, _rabbitexchangeName, bindingKey);
						var consumer = new QueueingBasicConsumer(channel);
						channel.BasicConsume(_rabbitQueueName, true, consumer);
						var encoding = Encoding.GetEncoding(_rabbitEncoding);

						#region Connect to Elastic Search Databasae

						var node = new Uri(_elasticUriString);
						var settings = new ConnectionSettings(node);
						settings.DefaultIndex(_elasticIndexName);
						var client = new ElasticClient(settings);

						#endregion

						while (true)
						{
							var ea          = consumer.Queue.Dequeue();
							var body        = ea.Body;
							var message     = encoding.GetString(body);
							var routingKey  = ea.RoutingKey;
							dynamic jsonmsg = JObject.Parse(message);
							var timenow     = DateTime.Now.ToString();

							jsonmsg.keys     = new JObject();
							jsonmsg.keys     = routingKey;
							jsonmsg.megadate = new JObject();
							jsonmsg.megadate = timenow;
							client.Index(jsonmsg);
							SaveToArray(jsonmsg);
						}
					}
				}
			}
		}

		private void SetUpTimer(TimeSpan executionTime)
		{
			DateTime current  = DateTime.Now;
			TimeSpan timeToGo = executionTime - current.TimeOfDay;
			if (timeToGo < TimeSpan.Zero)
			{
				return; //time already passed
			}
			this._timer = new Timer(x =>
			{
				this._dailyThread = new Thread(HandleDailyEvents)
				{
					Name         = "Daily",
					IsBackground = true
				};
				this._dailyThread.Start();
			}, null, timeToGo, Timeout.InfiniteTimeSpan);
		}

		/// <summary>
		/// This function adds new events to our dailyarray
		/// </summary>
		/// <param name="theEvent"></param>
		public void SaveToArray(JObject theEvent)
		{
			_dailyList.Add(theEvent);
		}

		/// <summary>
		/// This function handles our daily events and sets megadate to all events
		/// </summary>
		public void HandleDailyEvents()
		{
			_studentService.IncrementAllStudentsByOne();
			JArray listToWorkOn = new JArray();
			foreach (var i in _dailyList)
			{
				listToWorkOn.Add(i);
			}
			_dailyList.Clear();
			listToWorkOn.OrderBy(obj => obj["megadate"]);
			_messageService.WorkOnJson(listToWorkOn);
			SetUpTimer(new TimeSpan(04, 00, 00));
		}
	}
}
Info about Centris Retention:

This is the code which I (and my 3 project partners) wrote for our
Bachelors project in Computer Science at the Reykjavík University.

Our project was a contribution to an overall larger project called Centris.
Centris is an in-development inner web system for students and faculty.
The system has two parts to it. The LMS (Lecture Management System) part and
the SMS (Student Management System) part.
The LMS allows students to find anything pertaining to their studies, such as
their timetable, assignments, lectures, grades, etc. It also allows teachers
to do things like posting assignments, grades, lectures, etc.
The SMS is for the administrative staff, allowing them to manage registration,
scheduling, etc.

This Bachelors project, Centris Retention, is a system which is intended to
follow students' activity on the LMS site, and based on that, it evaluates the
probability of each individual student dropping out of their program.
A list of students, with their dropout probabilities is displayed on a page
in the SMS part of Centris.

Our project essentially had two parts to it: The backend and the frontend.
At the start of a semester, the backend fetches a list of students and
their class registrations.
The backend is designed to run on a server 24/7 and every night when traffic
to the LMS site is thought to be low, it will collect a log of all activities
for the past 24 hours from a message queue (RabbitMQ) and start to sort through
them all, calculating new dropout probabilites for every student.
The frontend is a part of the SMS site. It makes API calls to the backend to
get a list of all students, and their details. When a student is clicked,
it is possible to graphically view the progression of their dropout risk factor
for the previous month, along with brief explanation for changes in risk. 
It is also possible for a user (i.e. employee of the administrative office) 
to log when and if they contact a student offering help or warning them of 
their dropout risk.

This is a pretty extensive project, the backend was entirely built from scratch
while the frontend was developed as part of the larger SMS site code base.
Therefore this was a fine experience of both starting a large project from
square one, while also getting a chance to contribute to an overall larger, 
already existing project.
It is worth noting that this was intented to be a project which will be handed over
to a new group down the line. Our goal was simply to set up the framework for
this system. Since student dropout is such a nebulous and personality-based concept,
predicting it in any real sense would have to be done via artificial learning intelligence.
That in itself is complex enough to be it's own Bachelors project, so that is why
that wasn't something that we could get into implementing.

It is also worth noting that the actual repository for this project is located
elsewhere, in the greater context of the overall Centris project. However, 
since there's a lot of code there which I don't have the rights to, I can not
share that openly. I am simply uploading the code which my group wrote and delivered
upon completion of the project.
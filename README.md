#How to run the application?

1. First make sure that you have .net core sdk installed (or download it from here: https://www.microsoft.com/net/download)
2. Application is running use Microsoft SQL Server. You can either download it, or use docker version(which I probably recommend as it much less hassle)
3. To run Microsoft SQL server go to iseal/docker directory and in terminal execute "docker-compose up -d" (you have to have docker running).
4. After running database server, go to iSeal/src/iSeal.API
5. In terminal execute dotnet build
6. Now dotnet run
7. At the last line you will see what port you should be talking to (it's :5000 at my machine)
8. To test that everything works smoothly open your browser and go to http://localhost:5000/api/auth (on your machine port number might be different). If that works you should see "Hello user!" message.

PS: It happened on my environment that after running application I was unable to make any http requests using Postman. Request were completed before response was ready. To fix that I had to go to the link provided in 8. and accept in chrome unsafe connection.
PS 2: Database is seeding on each time that application is starting. Which means that on each start it clears all records in database

To view the database I'm using Microsoft SQL Server Management (although it's only for windows, when your running on Linux consider this [Microsoft SQL Operations Studio](https://docs.microsoft.com/pl-pl/sql/sql-operations-studio/what-is?view=sql-server-2017)
To connect to database credentials are:
server name: (local)
login: sa
password: Test1234!

#List of API calls:

(Post)
api/auth/register - Register new User. If Organization is empty creates user with no organization. If organization or user email already exists return 409 status code

body:
{
	"Email" : "ass@gmail.com",
	"Password" : "!Se4lP4ss",
	"PhoneNumber" : "123456789",
	"Organization" : "yourMom"
}

(Post)
api/auth/login - Returns JSON Web Token, and expire date (one hour from request).

body:
{
  "Email" : "admin@iseal.com",
  "Password" : "!se4lP4ss"
}

response
{
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AaXNlYWwuY29tIiwic3ViIjoiYWRtaW5AaXNlYWwuY29tIiwianRpIjoiNWExNjJlZDEtZWRhNy00Y2IyLTg3YzUtYmI5NTBmZjM3NTBjIiwiZXhwIjoxNTM3NTU0MDU3LCJpc3MiOiJpU2VhbCIsImF1ZCI6ImlTZWFsIn0.t1UugxQ2wbf9KXwmn27KzPDxJuL0vY645ySjhf6tEzU",
    "expiration": "2018-09-21T18:20:57Z"
}



(Post)
header: (authorization : Bearer {JWT value}
api/organization - Creates the organization

body:
{
	"Name" : "yourMom"
}

(Get)
header: (authorization : Bearer {JWT value}
api/organization/users/{organizationName} - returns list of users for organization
[
    {
        "email": "admin@iseal.com",
        "phoneNumber": null
    }
]



(Post)
header: (authorization : Bearer {JWT value}
api/seal/register - register seal for an organization that user belongs. Returns Bad Request if user does not belong to any organization
{
	"SyncKey" : "someMotherFuckingSecretStuffInGereGentlemen"
}

(Post)
header: (authorization : Bearer {JWT value}
api/seal/unlock - Check whether user can unlock the seal (if the organization that user belongs to can access it). Returns 401 Unauthorized if user can not.

body: 
{
	"Guid":"B75373F7-A07A-4D69-CCAC-08D61B07F88E"
}

response:
{
	"Guid":"B75373F7-A07A-4D69-CCAC-08D61B07F88E",
	"SyncKey":"someMotherFuckingSecretStuffInGereGentlemen"
}

(Get)
header: (authorization : Bearer {JWT value}
api/seal/list - retuns list of seals for a user in his organization (Bad Request if user does not have organization)

response:
[
    {
        "guid": "4e43d02c-4eb8-4b01-99e8-08d620710023",
        "syncKey": "asfbkqbriq2uh39fh103u^%#!@sfd"
    }
]
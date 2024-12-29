## BookStoreAPI
Project goal: become familiar with API tests and get some practice 

**Project Info:**
- API tests created for demo page: https://demoqa.com/swagger/
- coded with C#

**Run Settings need these properties to be specified:**
All the parameters below are created in the demo page
userName - user name, specified when you creating user in the demo page
userPassword - user password, specified when you create user in the demo page
userId - user Id, generated in demo page, when user is created.


**Test cases covered by API tests:**
- Get All Books - Success Scenario
- Add, Delete List Of Books To User - Success Scenario
- Add List Of Books To User - When NonExisting Isbn Number
- Add List Of Books To User - Invalid Authorization Header
- Add List Of Books To User - Invalid UserId
- Add Book To User - Book Is Already Added
- Get Book By Isbn Number - Success Scenario
- Get Book By Isbn Number - Isbn Not Found
- Delete All Books From User List - Invalid Authorization Header
- Delete All Books From User List - Invalid User Id
- Delete One Book From User List - Success Scenario
- Replace Book In User List - Success Scenario
- Replace Book In User List - Invalid Authorization Header
- Replace Book In User List - Invalid UserId
- Replace Book In User List - Isbn Number For Replace Not Found
- Replace Book In User List - Isbn Number To Replace Not Found




  

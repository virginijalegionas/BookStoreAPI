## BookStoreAPI
Project goal: become familiar with API tests and get some practice 

**Project Info:**
- API tests created for demo page: https://demoqa.com/swagger/
- coded with C#

**Run Settings need these properties to be specified:**
- appPackage - app property on the phone
- appActivity - app property on the phone
**Tip:** in mobile phone, open the app for which you want to find the appPackage and appActivity. Run command: `dumpsys window displays | grep -E ‘mCurrentFocus’`

![alt text](image.png)
like it's shown in the picture: green is appPackage, yellow is appActivity

- udid - the id of your phone connected to pc **Tip:** connect phone to pc, then run this comand in cmd: `adb devices`
- userName - demo eshop username, taken from the app
- userPassword - demo eshop user password, taken from the app

**Tip:** use this command to run tests in terminal, if you want to see more detailed test results: dotnet test -l "console;verbosity=detailed" --settings:.runSettings

**API tests handle these situations:**
- Product sorting
- Product page
- Basket operations
- Buy product with login
- Buy product without login
- Mandatory fields
- Login logout



  

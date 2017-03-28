# Acceptance testing with Selenium

Puppet web apps are C# apps with a thin presentation layer rendered in HTML5. Because of that, acceptance tests need to be executed in a web browser. If you would like to add automated acceptance testing to your project, there is no better tool than Selenium.

This page teaches how to use NUnit to run Selenium acceptance tests of a Starcounter Puppet web app. It is assumed here that you are using Visual Studio 2015, though it should also work with the older editions.

## What is Selenium

Selenium is a free (open source) automated testing suite for web applications across different browsers and platforms.

Selenium is not just a single tool but a suite of tools, each catering to different testing needs. The components that we will use are:

1. **Selenium WebDriver**, which is an API that controls the web browser. It is implemented in all major programming languages, including C#.
2. **Selenium RemoteWebDriver**, a Selenium WebDriver implementation that we will use, because it allows to execute the tests on the same or different machine. It is implemented in all major programming languages, including C#.
3. **Selenium Server Standalone**, a Java-based server that runs specific web browser client drivers. RemoteWebDrivers runs the tests through it.
4. **Selenium client drivers** for web browsers such as Firefox, Chrome and Edge.
3. **Selenium IDE**, a Firefox extension that allows to record the tests, so you don't have to write them manually.

You can learn more about Selenium at their [website (docs.seleniumhq.org)](http://docs.seleniumhq.org/docs/) or read one of the interesting blog posts:

- [Selenium Tutorial: Learn Selenium WebDriver](https://blog.udemy.com/selenium-ide-tutorial/)
- [Getting Started with Selenium IDE](http://www.softwaretestinghelp.com/selenium-ide-download-and-installation-selenium-tutorial-2/)

## What is NUnit

NUnit is a testing framework that has great integration with Visual Studio as well as continuous integration systems (TeamCity, etc). We will use NUnit to run our Selenium tests.

NUnit provides a [test runner](http://nunit.org/index.php?p=runningTests&r=2.6.4) (command line and built into Visual Studio), [attributes](http://nunit.org/index.php?p=attributes&r=2.6.4) to define the classes and functions that carry the testing code, [assertions](http://nunit.org/index.php?p=assertions&r=2.6.4) that test the actual values.

You can learn more about NUnit at their [website (nunit.org)](http://nunit.org/index.php?p=quickStart&r=2.6.4) or read one of the interesting blog posts:

- [C# Unit Test Tutorial](http://www.rhyous.com/programming-development/csharp-unit-test-tutorial/)
- [Unit testing with .NET](http://www.developerfusion.com/article/84847/unit-testing-with-net/)
- [Learning NUnit In Easy Way For Beginners](http://learnseleniumtesting.com/learning-nunit-in-easy-way-for-beginners/)

## Create a test project

It is recommended to keep the testing project in the same solution as the tested project. Note that they are two different projects.

In the following example, we will add acceptance tests to Launcher. Let's create a new test project in Visual Studio, by clicking <kbd>Add New Project</kbd> → <kbd>Visual C#</kbd> → <kbd>Test</kbd> → <kbd>Unit Test Project</kbd>.

Call the new project "Launcher.AcceptanceTest". We will use `Launcher\test\` as the project location.

![add new project](/assets/2016-04-01-13_03_00-Add-New-Project.png)

## Install required packages

Open the newly created test project. Now, we need to install a bunch of libraries mentioned above.

Open the package manager (<kbd>Tools</kbd> → <kbd>NuGet Packet Manager</kbd> → <kbd>Packet Manager Console</kbd>).

**Important:** In the console, choose your test project from the "Default project" dropdown.

![install required packages](/assets/2016-04-01-13_05_38-Launcher-Microsoft-Visual-Studio.png)

Run the following commands in the console to install the required dependencies:

```
Install-Package Selenium.WebDriver
Install-Package Selenium.Support
Install-Package NUnit -Version 3.*
Install-Package NUnit.ConsoleRunner -Version 3.*
Install-Package NUnit3TestAdapter
```

## Run your first test

Clone KitchenSink repo from the StarcounterApps organisation on GitHub.

Build your test project. If it builds correctly, you should see this:

![Seleninium result screenshot](/assets/2016-04-01-13_34_52-Launcher-Microsoft-Visual-Studio.png)

Now, the only thing left to do is to run that test! In the Test Explorer, click on the "Run All" button. If it works well, you should see your tests passing.

![test explorer](/assets/2016-04-01-13_40_22-Launcher-Microsoft-Visual-Studio.png)

## Running in multiple browsers

We don't want to test only Firefox. Because of that, you should use Selenium RemoteWebDriver, which adds a layer of abstraction that runs tests in multiple browsers, possibly on remote machines.

### Install Selenium Standalone Server and browser drivers

To make this happen, you will need to install some additional software.

- Download and install Java, required by Selenium Standalone Server
- Create a directory `C:\Selenium`
- Download the following files from [Selenium Downloads](http://docs.seleniumhq.org/download/):
  - Selenium Standalone Server (`selenium-server-standalone-3.*.jar`)
  - Gecko Driver (`geckodriver.exe`)
  - Microsoft Edge Driver (`MicrosoftWebDriver.exe`)
  - Google Chrome Driver (`chromedriver.exe`)
- Put the `jar` file as well as 3 `exe` files directly in `C:\Selenium`

Open your <code>Properties</code> in the <code>Tests</code> project. Go to <code>Reference Paths</code>, enter C:\Selenium and click Add Folder.

### Use BaseTest class to run tests in multiple browsers

BaseTest is a helper class that makes it easier to test multiple browsers. The source code is available:

- BaseTest helper class [here](https://github.com/StarcounterApps/KitchenSink/blob/master/test/KitchenSink.Tests/Test/BaseTest.cs)
- Using of BaseTest class [here](https://github.com/StarcounterApps/KitchenSink/blob/master/test/KitchenSink.Tests/Test/SectionBoolean/CheckboxPageTest.cs)

When you rebuild the test project now, you should see each test for every browser.

The final setup looks like this:

![visual studio selenium screenshot](/assets/2016-04-01-13_51_26-Launcher-Microsoft-Visual-Studio.png)

Before you can execute the tests, start Selenium Server Standalone by calling `java -jar selenium-server-standalone-3.*.jar`.

## Wait for asynchronous content

There is one common pitfall when writing Selenium tests. The test is executed with disregard of the asynchronously loaded content. This means that your tests need to explicitly wait for UI elements to appear before running any actions or assertions on them.

It is a good practice to always wait:

- Wait for a text element to be present before you check the content of that element

Example can be find [here](https://github.com/StarcounterApps/KitchenSink/blob/master/test/KitchenSink.Tests/Test/SectionString/TextareaPageTest.cs). 
Please take a look at TextareaPage_WriteToTextArea() test and line #37. 
Method WaitForText() is use to wait for text in "TextareaInfoLabel" for maximum 5 second. Then assert is true and test pass.

- Wait for a button to be present before you click on that button

Example can be find [here](https://github.com/StarcounterApps/KitchenSink/blob/master/test/KitchenSink.Tests/Test/SectionNumber/ButtonPageTest.cs)
As you can see in line #35 in ButtonPage_RegularButton() test we use one of the helper method called WaitUntil. 
As parameter we use Displayed property for the button. The test will wait until ButtonInlineScript is displayed. Default maximum wait time is 10 second.

- Wait for presence of an input field before typing in it and wait for text to be present in label

Example can be find [here](https://github.com/StarcounterApps/KitchenSink/blob/master/test/KitchenSink.Tests/Test/SectionString/TextPageTest.cs)
In line #31 in TextPage_TextPropagationOnUnfocus() test we use WaitUntil helper method again to wait for input to be displayed. 
In line #34 WaitForText() is used to wait for text in label for maximum 5 second.



## Sample test suites

Some of the Starcounter's sample apps come with acceptance test suite. We run tests every night to make sure that we keep the good quality.

The [KitchenSink](https://github.com/StarcounterApps/KitchenSink) sample app includes a Selenium test case for every UI pattern that is presented in that app. You can learn from the test project (in the `test` directory), how to achieve Selenium testing of particular actions, such as button clicks, page changing, typing in forms, etc.

The [Launcher](https://github.com/StarcounterApps/Launcher) prefab app includes Selenium test of using Launcher with two mock applications (called "Launcher_AcceptanceHelperOne" and "Launcher_AcceptanceHelperTwo"). The test include executing JavaScript code on a page to scroll a DIV element and then checking the scroll position.

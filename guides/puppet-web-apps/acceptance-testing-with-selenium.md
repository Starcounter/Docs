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
Install-Package NUnit -Version 2.6.4
Install-Package NUnitTestAdapter
Install-Package NUnit.Runners -Version 3.2.0
```

## Record your first test

"Record" the test? Yes, you read this right! While you can also write the test code manually using Selenium's domain-specific language, it is way more effective to record it directly from a Firefox extension.

Make sure you have Selenium IDE extension installed in Firefox. The instructions are here: [http://www.seleniumhq.org/projects/ide/](http://www.seleniumhq.org/projects/ide/).

Make sure that the project that you want to test is already running. For me it means, that Launcher is started and accessible at [http://localhost:8080/launcher](http://localhost:8080/launcher).

To record the first test, go to Firefox, and click on the Selenium IDE icon.

![record first test location](/assets/2016-04-01-13_24_19-Mozilla-Firefox.png)

When the Selenium IDE opens, it automatically starts recording. 

![selenium IDE](/assets/2016-04-01-13_25_24-Mozilla-Firefox.png)

Now, you need to focus your main Firefox window again. Type in the URL of your app, press ENTER and wait until the page loads.

When the page is loaded, right click anywhere on the page. In the context menu, select <kbd>Show All Available Commands</kbd> → <kbd>waitForTitle</kbd>. This records an action of waiting for a page title to appear in the window title bar. This will make a good first test to see if your app loads correctly.

Now go back to Selenium IDE and press the red circle to stop recording. The recorded output should look like this:

![start recording](/assets/2016-04-01-13_29_20-Launcher.png)

Now, you can save this recording as a `.cs` file. In Selenium IDE, click <kbd>File</kbd> → <kbd>Export Test Case As…</kbd> → <kbd>C# / Nunit / WebDriver</kbd>.

In the file dialog, go to your test project directory and save the file as `PageLoads.cs`

## Run your first test

Now, import the recorded file to your project in Visual Studio. Right click on the project directory, select <kbd>Add</kbd> → <kbd>Existing item…</kbd> and pick the file `PageLoads.cs`. Open the file.

Still in Visual Studio, open the Test Explorer window pane, by clicking on <kbd>Test</kbd> → <kbd>Windows</kbd> → <kbd>Test Explorer</kbd>.

Build your test project. If it builds correctly, you should see this:

![Seleninium result screenshot](/assets/2016-04-01-13_34_52-Launcher-Microsoft-Visual-Studio.png)

Now, the only thing left to do is to run that test! In the Test Explorer, click on the "Run All" button. In the following seconds, put your hands up from your mouse and keyboard, because Selenium will take control of your system and perform the test. If it works well, you should see your tests passing.

![test explorer](/assets/2016-04-01-13_40_22-Launcher-Microsoft-Visual-Studio.png)

## Running in multiple browsers

We don't want to test only Firefox. Because of that, you should use Selenium RemoteWebDriver, which adds a layer of abstraction that runs tests in multiple browsers, possibly on remote machines.

### Install Selenium Standalone Server and browser drivers

To make this happen, you will need to install some additional software.

- Download and install Java, required by Selenium Standalone Server
- Create a directory `C:\selenium`
- Download the following files from [Selenium Downloads](http://docs.seleniumhq.org/download/):
  - Selenium Standalone Server (`selenium-server-standalone-3.0.0-beta3.jar`)
  - Gecko Driver (`geckodriver.exe`)
  - Microsoft Edge Driver (`MicrosoftWebDriver.exe`)
  - Google Chrome Driver (`chromedriver.exe`)
- Put the `jar` file as well as 3 `exe` files directly in `C:\selenium`

Open your <code>Properties</code> in the <code>Tests</code> project. Go to <code>Reference Paths</code>, enter C:\selenium and click Add Folder. 

### Edit the tests to run in multiple projects

Now, add this small file to your test project. Call it `WebDriverFactory.cs`. This is a helper class that makes it easier to test multiple browsers. The source code is available [here](https://github.com/StarcounterSamples/KitchenSink/blob/master/test/KitchenSink.Tests/WebDriverFactory.cs):

```cs
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;

class WebDriverFactory {
    private static Uri remoteWebDriverUri = new Uri("http://127.0.0.1:4444/wd/hub");

    public static IWebDriver Create(string browser) {
        DesiredCapabilities capabilities;
        IWebDriver driver;

        switch (browser) {
            case "chrome":
                capabilities = DesiredCapabilities.Chrome();
                driver = new RemoteWebDriver(remoteWebDriverUri, capabilities);
                break;
            case "internet explorer":
                InternetExplorerOptions options = new InternetExplorerOptions();
                options.IgnoreZoomLevel = true;
                capabilities = (DesiredCapabilities)options.ToCapabilities();
                driver = new RemoteWebDriver(remoteWebDriverUri, capabilities, TimeSpan.FromSeconds(10));
                break;
            case "edge":
                capabilities = DesiredCapabilities.Edge();
                driver = new RemoteWebDriver(remoteWebDriverUri, capabilities);
                break;
            default:
                capabilities = DesiredCapabilities.Firefox();
                driver = new RemoteWebDriver(remoteWebDriverUri, capabilities);
                break;
        }

        return driver;
    }
}
```

Now, we need to adapt the test file to make use of it.

In the file you created in the previous step (`PageLoads.cs`), locate the following line:

```cs
driver = new FirefoxDriver();
``` 

Change it to:

```cs
driver = WebDriverFactory.Create(this.browser);
```

Now, what is `this.browser`? This is a fixture parameter that carries the name of the browser used to execute the test.

What this means is that you need to change every instance of :

```cs
[TestFixture]
```

To be:

```cs
[TestFixture("firefox")]	
[TestFixture("chrome")]
[TestFixture("edge")]
```

Finally, add this constructor somewhere on top of the class. It stores the fixture parameter in the `this.browser` variable:


```cs
private string browser;

public PageLoads(string browser) {
    this.browser = browser;
}
```

When you rebuild the test project now, you should see 3 tests in the Test Explorer, each test for every browser.

The final setup looks like this:

![visual studio selenium screenshot](/assets/2016-04-01-13_51_26-Launcher-Microsoft-Visual-Studio.png)

Before you can execute the tests, start Selenium Server Standalone by calling `java -jar selenium-server-standalone-3.0.0-beta3.jar`.

Now, press "Run All" and relax. This will take a while as the system runs your test in three different browsers!

![test explorer screenshot](/assets/2016-04-01-15_38_44-Launcher-Microsoft-Visual-Studio.png)

## Wait for asynchronous content

There is one common pitfall when writing Selenium tests. The test is executed with disregard of the asynchronously loaded content. This means that your tests need to explicitly wait for UI elements to appear before running any actions or assertions on them. 

It is a good practice to always wait:

- wait for a text element to be present, before you check the content of that element
- wait for an input field to be present, before you type in that input field
- wait for a button to be present, before you click on that button

The following code sample from KitchenSink's [TextPageTest.cs](https://github.com/StarcounterSamples/KitchenSink/blob/master/test/KitchenSink.Tests/Tests/TextPageTest.cs) shows how to: 

1. wait for presence of an input field before typing in it
2. wait for the asynchronous response from the server with derivative result

```cs
[Test]
public void TextPropagationOnUnfocus() {
    driver.Navigate().GoToUrl(baseURL + "/Text");
    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    IWebElement element = wait.Until(_driver => _driver.FindElement(By.XPath("(//input)[1]")));
    
    var label = driver.FindElement(By.XPath("(//label[@class='control-label'])[1]"));
    var originalText = label.Text;
    driver.FindElement(By.XPath("(//input)[1]")).Clear();
    driver.FindElement(By.XPath("(//input)[1]")).SendKeys("Marcin");
    driver.FindElement(By.XPath("//body")).Click();
    
    wait.Until((x) => {
        return !label.Text.Equals(originalText);
    });
    Assert.AreEqual("Hi, Marcin!", driver.FindElement(By.XPath("(//label[@class='control-label'])[1]")).Text);
}
```


## Sample test suites

Some of the Starcounter's sample apps come with acceptance test suite. We run tests every night to make sure that we keep the good quality.

The [KitchenSink](https://github.com/StarcounterSamples/KitchenSink) sample app includes a Selenium test case for every UI pattern that is presented in that app. You can learn from the test project (in the `test` directory), how to achieve Selenium testing of particular actions, such as button clicks, page changing, typing in forms, etc.

The [Launcher](https://github.com/StarcounterPrefabs/Launcher) prefab app includes Selenium test of using Launcher with two mock applications (called "Launcher_AcceptanceHelperOne" and "Launcher_AcceptanceHelperTwo"). The test include executing JavaScript code on a page to scroll a DIV element and then checking the scroll position.
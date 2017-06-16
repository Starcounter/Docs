# Blending Tutorial Introduction

Blending is the process of making different apps look and work as one.

This tutorial will teach you how to blend two independent apps.

## Requirements

To follow along, you'll need to fulfill the Starcounter [system requirements](https://starcounter.io/download/#system-requirements). 

The required Starcounter version is 2.3.1 or later. Download it from the release candidate channel on the [download page](http://downloads.starcounter.com/download).

To build the apps, you need to have Visual Studio 2015 or 2017 installed. The community edition of Visual Studio can be downloaded for free on [visualstudio.com](https://www.visualstudio.com/downloads/).

## The Apps

The two apps that we will blend are PetList and MedicalRecordProvider. 

PetList is an app displaying a list of pets and the details of each pet.

![Pet list](/assets/PetList.gif)

MedicalRecordProvider is a list of medical examinations and a summary for those examinations. You can filter the content in the list like this: `http://localhost:8080/MedicalRecordProvider?name=fluffy`.

![Medical record provider](/assets/MedicalRecordProvider.gif)

## Result

The result of combining these two apps with blending is a medical system for pets that allows us to get a list of pets, see the summary of their medical records and also view the details of each pet's medical record.

![Pet Med System Result](/assets/ResultPetMedSystem.gif)

## Source Code

The source code can be found in the [BlendingTutorial](https://github.com/StarcounterApps/BlendingTutorial) repository on GitHub. Every commit represents one step in the tutorial:

* Create two simple apps
* Adapt to declarative shadow dom
* Create shared data model
* Server-side blending
* Client-side blending

## First Step

The first step is to create two simple apps. To do this, clone the [BlendingTutorial](https://github.com/StarcounterApps/BlendingTutorial) repo and checkout the first commit or download the zip of that commit.

```git
git clone https://github.com/StarcounterApps/BlendingTutorial.git
git checkout ed05d5c01250b1e2db8e13073cd4c17527d7d000
```

Build and run the apps by executing the `build.bat` and `run.bat` scripts. 

```git
build.bat
run.bat
```

In git bash, use `./build.bat` and `/run.bat` instead. 

They should now be available at `localhost:8080/PetList` and `localhost:8080/MedicalRecordProvider`. 
# LootlockerInventorySystem

An inventory system built with SOLID principles in mind. This connects to a backend structure called lootlocker available here https://www.lootlocker.io/ 
Code is sectioned into systems that can be reused in other projects.
Systems can be found in the scripts folder and they include :

## Installing Lootlocker
After pulling this project. You will need to install lootlocker from the package manager by following these instructions 

### Using GIT
Before you start, make sure you have git installed on your computer. Open the Unity editor and navigate to the Package Manager by clicking on Window and then Package Manager.

Click on the + at the top of the Package Manager window and select Add package from git URL.

Enter the URL below into the text box and click Add. https://github.com/LootLocker/unity-sdk.git

The SDK is now installed in your project, and you are ready to configure the SDK and make your first API calls.

### Using the Entire Repo
If you do not have git installed on your computer. You can install the SDK by downloading the entire repository and pasting in the packages folder of your project. You can download the entire repo by clicking Code and Download Zip

The SDK is now installed in your project, and you are ready to configure the SDK and make your first API calls.

## DraggableSystem 
Easily drag and drop ui elements and also dr

## ListManagerSystem 
Easiy create lists of UI elements, instead of always instatiaing children when you intend to create a list. It supports a standard list with a parent and children as well as a list where the items also have child items. Good examples can be found with the inventory loadout and inventory list. Supports reusablity, used everywhere a list of ui elemtents needs to be generated. It also supports knowing everything about the list, when children are destroyed, count and much more. Can be easily extended based on requirements. 


## ServerRequestsPersistSystem
Work in progress - easily make server calls persitent. This means you can wrap your server calls so they must happen in the background. So you dont need to worry about your server calls failing. If they fail, the system will keep trying it in the background until it is sent. If the application is closed before it is successful. The system saves itself and reloads it so it can continue retrying in the background.


## TagNotificationSystem
Simple system to notify gameobjects when different activities occur. Similar to SendMessage in unity. But supports categories and tags.

## TextureDownloadingSystem
A hands off method of handling texture downloading from the server. Especially useful for projects that have images stored on the backend. Instead of always writing couroutines to download images when you need them. You can pass this to the system, and it downloads and makes images available to you when necessary. It also makes sure the same image is not downloaded twice by caching it locally. Also supports local images, so you dont need to have a different system for images in the project.

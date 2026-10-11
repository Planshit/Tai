# Tai

👻 Track how much time you spend using **applications** and browsing **websites** on Windows.

<img src="index.jpg" width=600 />

**Advertising space available.** Contact: `bmFjYXQ0YjEx` (Base64 encoded). Please include a note explaining your inquiry.

## Getting Started

#### Requirements

Before using Tai, you may need to install [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework) `4.8` or later. (Generally, this is not necessary on Windows 10 or later.)

If Tai does not respond when you launch it, click the link above to download and install the required framework.

#### Usage

1. Download the precompiled executable archive from [Releases](https://github.com/Planshit/Tai/releases). It is recommended to choose the version marked `Latest`. Scroll down to the `Assets` section and download `Taix.x.x.x.zip`.
2. Extract the downloaded archive to a suitable location (preferably not your desktop or system drive). Open the extracted folder, locate `👻Tai.exe`, and launch it. Once successfully started, you will see a 👻 icon in the system tray.
3. To use the **Website Browsing Statistics** feature, you must install the appropriate browser extension and enable the feature under **Settings > General > Features**. See the [Browser Extension Installation Guide](https://github.com/Planshit/Tai/discussions/279).
4. Run Tai as an administrator; otherwise, it may not be able to track certain applications.

#### Uninstallation

1. Disable automatic startup in Settings if you have enabled it.
2. Delete all the files to complete the uninstallation.

#### Basic Operations

Double-click the icon to open the main window. Right-click the icon to display the menu.

## Learn More

#### Why Use Tai?

Tai helps you understand how you spend your time so you can plan your activities more effectively. Or simply review how much time you spent slacking off each week.

~~Work is just a way to earn a salary; slacking off is the real way to make money.~~

#### Application Whitelist

Tai supports tracking only applications included in a whitelist. You can enable this feature under **Settings > Behavior**.

#### Process Association

If you want to track the usage time of applications running on other monitors while working with multiple displays, you can add their processes to an association list in Settings.

Once associated, using any application in the list will also update the recorded usage time of the other applications in that list, provided those applications are running. They can be in the background or not currently focused.

Each application can belong to only one association group and cannot be associated more than once.

#### Filtering Applications and Websites

Under **Settings > Behavior**, you can filter out applications or websites you do not want to track using process names, URLs, or regular expressions.

#### Data Storage and Export

Tai stores tracking data in an unencrypted `SQLite` database located at `Data\data.db` in the application's running directory.

You can export your tracking data in `.xlsx` and `.csv` formats through Settings.

#### Sleep Detection

Tai can detect when you step away from your computer to some extent and stop tracking accordingly.

You can disable this feature under **Settings > Behavior** to enable uninterrupted tracking.

#### Automatic Categorization

Applications can be automatically categorized based on their installation or executable directory.

When adding or editing a category in Category Management, you can enable or disable directory matching.

For example, if you enable directory matching for `Category A` and add the directory `C:\`, all applications located on the C drive will automatically be assigned to `Category A`.

#### Network Connectivity and Privacy

Tai makes no network requests other than those required when you manually check for updates or upgrade the software through Settings.

Tai does not collect or upload any of your information.

## ❤️ + 👻

Support from users, whether moral or financial, helps keep open-source software development going. If Tai has been useful to you, consider showing the developer some support!

#### Buy Me a ☕

<img src="https://github.com/noberumotto/noberumotto/raw/master/wechat.jpg" width="256px" /> <img src="https://github.com/noberumotto/noberumotto/raw/master/alipay.jpg" width="256px" />

[About & Contact →](https://github.com/noberumotto/noberumotto/blob/master/about.md)


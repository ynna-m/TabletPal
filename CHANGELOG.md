## [v0.5.0] - 04.15.2026
### First Beta Release!
Hello! This is the first beta release for Tablet Pal. Tablet Pal is a fork of Tablet Friend.
Renamed it, since the initial plan was to make this cross-platform and still stick to Tablet Friend, but alas, that didn't exactly work out. If anyone can do that, please feel free to do so. So I decided to rename it Tablet Pal since I work mostly on Linux, that if I do make any future updates on this, it is going to diverge pretty much from the original.

Other than the changes indicated below, so far in this initial beta release, it should work similarly to Tablet Friend with a some small caveats on the Changes below:

### Changed
- The biggest change is switching from WPF to Avalonia, so there are some slight UI aesthetic changes particularly the Tray Icon menu.
- Since this is built for Linux, docking doesn't exactly work the same as Windows, e.g. Windows and the desktop Taskbar do not move about.
- Only works with X11 Display Server Protocols. I don't think it works on Wayland. It might with XWayland, but it is untested there.
- There's a prompt that will appear when you run TabletPal for the first time. It asks you if you want to extract the files directory from the AppImage, clicking __No__ will not make the AppImage run. The only other way for the AppImage to run if you click __No__ is to migrate your files directory and settings.yaml from a previous installation, and set first_launch: false in settings.yaml.
- After the prompt from above, since there is no AppData directory for Linux, it will instead just ask if you want to install TabletPal to your home directory.
- I have also removed autohiding toolbar. It was a little too complicated to implement as of the moment on Linux

If you found any errors or bugs in the app, please message me via the contact form at my website: `https://ynnadev.com/contact-me`
And I will add that as an issue in the board as soon as I confirm it.
---

## [v0.4.1] - 04.15.2026
Hello!
This is a test trial release v2 using Github Workflow Actions. Please ignore this first release. Testing the body_path in Github actions.
    
---

## [v0.4.0] - 04.15.2026
Hello!
This is a test trial release using Github Workflow Actions. Please ignore this first release. 

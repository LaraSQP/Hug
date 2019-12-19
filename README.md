# Hug selected text with custom tags via 'Quick Actions'

[![Build status](https://ci.appveyor.com/api/projects/status/m90gorwsrp7d3ktf?svg=true
)](https://ci.appveyor.com/project/LaraSQP/hug)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](license.txt)

This extension makes it possible to quickly surround selected text with custom tags via 'Quick Actions' in Visual Studio 2019.

The following tags are included by default:

 - Double-quotes `""`
 - Single-quotes `''`
 - Parentheses `()`
 - Square brackets `[]`
 - Curly brackets `{}`
 - Markup symbols `<>`
 - Number symbols `##`
 - Asterisks `**`


## Setup

Install from the `Open VSIX gallery` via the `Extensions -> Manage Extensions` menu (you might need to add the feed, see [this](http://vsixgallery.com/guide/feed/)) or [download the latest CI build]() as a `VSIX` package and install it manually.

- **Note that** the `releases` tab at `GitHub` will either be empty or have out-of-date releases.

## Getting Started

- Once installed, the `Extensions` menu will show the entry ![hug](https://user-images.githubusercontent.com/12540983/71141321-1c44ee00-2257-11ea-9324-098349e769dc.png) `Hug`, as shown below:

![extensions](https://user-images.githubusercontent.com/12540983/71141537-c6bd1100-2257-11ea-9d90-0ba66355c851.png)

- Clicking on the menu item ![hug](https://user-images.githubusercontent.com/12540983/71141321-1c44ee00-2257-11ea-9324-098349e769dc.png) `Hug`, brings up the `Hug's tag manager` where it is possible to add, delete, and change the position of the tags in the `Quick Actions` suggestions menu (i.e., the yellow lightbulb).

- Enter a left tag and a right tag in the corresponding textboxes and click on the `Add` button to create a new pair of tags. Duplicate pairs of tags are not allowed and both left/right tags are required.

![manager](https://user-images.githubusercontent.com/12540983/71141671-45b24980-2258-11ea-9efb-af38a29714a4.jpg)

- Click on `Delete`, or press the `delete`/`backspace` key, to remove the selected tag(s).

- If the checkbox `Sort by use` is checked, the tags will sorted from most used to least used and will appear so in both the manager and in the `Quick Actions` suggestions menu as shown below. Note that this is a live count that reflects ongoing usage. Thus, entries might swap places too often for certain personality types.

- To remedy this improbable yet potential vexation, it is possible to fix the arrangement of the tags as deemed suitable in order to meliorate the overall wellbeing and, indeed possibly, to ensure the survival itself of one's sanity. This is accomplished by removing the check mark from the checkbox `Sort by use`, which will enable the buttons `Move up` and `Move down`. Proceed at this juncture by selecting those tags that require relocation and click away as desired.

![quickactions](https://user-images.githubusercontent.com/12540983/71142027-7181ff00-2259-11ea-9317-51fc75582cf9.jpg)

- If there are more than 5 tags, they will be placed in the `More Hugs...` submenu to avoid overcrowding the `Quick Actions` menu.

## Limitations

It is safe to assume that **Hug** has some limitations since it has only been tested with `c# solutions` and on just a few of machines.
  
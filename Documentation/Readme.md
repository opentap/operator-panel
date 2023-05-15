# Operator Panel

The Operator Panel is a user interface plugin for Test Automation, designed to streamline manufacturing tests.

![operator ui](images/operator-ui.png)

## Table of Contents

1. Overview
2. Licensing
3. Installation
4. Usage
5. Customization
6. Source code and license
7. For more information


## 1. Overview

Manufacturing environments often require a streamlined and user-friendly experience to increase efficiency and reduce the chance of errors. The Operator Panel provides an intuitive solution, allowing for simultaneous execution of any number of parallel tests. All panels run the same test plan, but each has a separate set of external parameters that are set individually. In 'focus mode', there is no way to modify or affect the test plan run, except for a few intended methods.

The Operator Panel plugin can be used out-of-the-box, but the source code is also provided as a basis or example for creating new panel implementations tailored to your specific needs.


## 2. Licensing

To execute and view test plans using custom panels in Test Automation (Editor.exe), you will need a TAP_Engine license.

If you also need to edit test plans using standard Test Automation features, a TAP_Editor license is required.

Note that if you only need to execute test plans, the TAP_Engine license is sufficient.

## 3. Installation

- Install the Operator Panel plugin using the Test Automation Package Manager or using the command line interface:
  - `tap package install "Operator Panel"`
- Start Test Automation.

## 4. Usage
- The basic panel can be opened from the View menu.

![view menu](images/view-menu.png)

- The operator panel can be docked next to the test plan panel like this:

![view menu](images/operator-test-plan.png)


To make the panel aware of you DUT, parameterize the DUT property unto the test plan.

Results seen in the operator panel are properties from the test plan steps which are marked with the `[Result]` attribute. How to manage this can be seen in the example / demonstration plugin.

## 4.1 Focus Mode

Focus mode minimizes the number of interactions with Test Automation, reducing the likelihood of operators performing unintended actions.

For optimal use of the Operator Panel in a manufacturing setting, follow these steps:

1. Remove all unnecessary panels from the view.
2. Dock the Operator Panel in the center of the user interface.
3. Maximize the window and enter focus mode by pressing the ALT+Shift+Enter keyboard shortcut.
4. Save this preset by pressing CTRL+ALT+Shift+S.

We recommend creating both a 'developer' and an 'operator' preset, which can be easily switched between using the ALT+Shift+[1234] keyboard combination. See the preset menu for details.

![Presets menu](images/presets-view.png)


## 5. Customization
Different manufacturing environments may require different operator panel implementations. For example, you may need to customize the panel's layout, add or remove certain features, or integrate it with other tools.

The current implementation of the Operator Panel may make assumptions that are not optimal for every environment. Hence, we provide the source code as a basis or example for creating your own implementation. You can tailor the code to meet your specific needs and adapt it to your unique situation.

We encourage you to explore the source code and experiment with different customizations. Don't hesitate to share your modifications with the OpenTAP community or seek help from our support forums.

For more information on customizing the Operator Panel, check out our documentation and tutorials.

## 6. Source Code and License
The source code for the Operator Panel is provided under the MIT license.

## 7. For more information
Here are some resources to help you learn more about the Operator Panel and get support:

- License: To obtain a TAP_Engine license, please contact your Keysight representative. For more information, you can visit https://www.keysight.com/zz/en/product/KS8000B/pathwave-test-automation-deployment-system.html/ 

- Source code: The source code for the Operator Panel can be found on GitHub at https://github.com/opentap/operator-panel. Explore the code and experiment with different customizations to create your own implementation tailored to your specific needs.

- OpenTAP website: Visit https://opentap.io to learn more about OpenTAP and its features.

- Support forum: Join the OpenTAP community at https://forum.opentap.io to ask questions, share your experiences, and get help from other users and experts.

# Umbraco.PatternLib

## Atomic Design
This is our approach to a simple system to streamline use of Atomic Design principles in Umbraco sites.

For an intro to Atomic Design see http://bradfrost.com/blog/post/atomic-web-design/

## Pattern Library

This initial version of Umbraco.PatternLib adds a Pattern Library viewer (concept adapted from http://patternlab.io/) to any Umbraco site.

Once installed & configured you will be able to view your Pattern Library at http://your.website/patternlib

## Installation & Setup

1. Install package via nuget : https://www.nuget.org/packages/Endzone.Umbraco.PatternLib.Core/
2. Add web.config AppSetting : <add key="PatternLib.enable" value="true"/>
3. View http://your.website/patternlib

## How to use

The assumption is that your front end coding process will take place outside the Umbraco project, the output of that will be a series of files containing discrete static markup for your patterns.

* Place "pattern" .htm files in \Views\_patternlib\static
* You can use any folder structure to organise and group your patterns (eg atoms, molecules etc)
* Use any naming convention for folders and files (although files must be .html)
    * To control display order in the viewer we recommend prefixing folders/files with numbers (eg 01-atoms)
* The pattern .htm files should contain ONLY the html, classes etc needed for that pattern
* Use \Views\_patternlib\static\_PatternMaster.cshtml to bring in CSS, JS etc
 
Here is a simple example : https://github.com/EndzoneSoftware/Umbraco.PatternLib/tree/master/Umbraco.PatternLib.TestSite/Views/_patternlib-example/static 


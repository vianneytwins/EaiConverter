[![Build Status](https://travis-ci.org/vianneytwins/EaiConverter.svg?branch=master)](https://travis-ci.org/vianneytwins/EaiConverter)
# EaiConverter
This project is an attempt to convert EAI Xml description format into source code (C#, Vb.net, C++...)
 Supported input format are Tibco Business Work 5.7 and the only real tested output is C#

## Why this project ?
 Because lots of people tend to put too much business logic in EAI and they are not designed for it. Your EAI tends to be more and more difficult to maintains

## Technical choices made
 Convert most of the XML / XSD object in POCO

## What is left to you once the project is converted ?
 This tools generate code but no solution (sln file  or prj file), so you need to create one.
 It expects also that you add your own injection framework (Ninject, Unity). Services, Processes and other service type classes will need to be added in your module(s) files.(We will add one example later)

 you will also need to add your own implementation of the IDataAccessFactory to connect to database (We will add one example later)
 why ? Because we thought that most people have existing project in which they will import those classes, so no need to rewrite them. 
 
## Supported activity types are
- com.tibco.plugin.jdbc.JDBCCallActivity
- com.tibco.plugin.jdbc.JDBCUpdateActivity
- com.tibco.plugin.jdbc.JDBCQueryActivity
- com.tibco.plugin.xml.XMLParseActivity
- com.tibco.plugin.mapper.MapperActivity
- com.tibco.pe.core.CallProcessActivity
- com.tibco.pe.core.AssignActivity

## main issues
Xpath Conversion Generation
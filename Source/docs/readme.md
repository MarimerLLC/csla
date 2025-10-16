# Doxygen Documentation Generation

This document provides an overview of how to generate documentation using Doxygen for the CSLA .NET project.

## Prerequisites

Before generating the documentation, ensure you have the following installed:

- [Doxygen](https://www.doxygen.nl/)
- Clone the [CSLA .NET repository](https://github.com/MarimerLLC/csla)

## Generating Documentation

To generate the documentation, run the following command _one level above_ the root directory of the project:

```bash
doxygen -q csla/Source/docs/doxyfile
```

This will generate the documentation based on the configuration specified in the `doxyfile`. The output will be placed in the directory specified by the `OUTPUT_DIRECTORY` setting in the `doxyfile`. The default is an `output` directory at the level where you ran the command.

```text
csla/
output/
```

The generated documentation will include HTML files that you can open in a web browser to view the API documentation.

## Viewing the Documentation

To view the generated documentation, open the `index.html` file located in the `output/html` directory in your web browser.

```text
output/html/index.html
```

This will display the main page of the generated documentation, where you can navigate through the various classes, files, and namespaces documented by Doxygen.

## Landing Page Content

The `docs-readme.md` file serves as the landing page for the generated documentation. It provides an overview of the project, instructions for generating the documentation, and guidance on how to navigate and use the generated API documentation. This file is written in Markdown and is converted to HTML by Doxygen to create the main page of the documentation.

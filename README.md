# Cloud-Native Bike Shop (.NET Core)

This project includes hands-on labs that build out an example "bike shop"
API. For the Spring Boot version of this bike shop API, see the [BikeShopAPISpring repo](https://github.com/Magenic/BikeShopAPISpring).
For the UI that works against both the Spring-based and .NET Core-based APIs, see
[BikeShopUIAngular repo](https://github.com/Magenic/BikeShopUIAngular.

## Requirements

To do each of the labs, you should have the following installed and configured:

1. An IDE or text editor, such as:
   1. VS Code
   1. Atom
1. .NET Core 2.2.x
1. The [CloudFoundry CLI](https://docs.run.pivotal.io/cf-cli/install-go-cli.html).

## Overview

The project works through building the API in the following steps:

1. Setup
1. Connectors
1. Configuration
1. Management
1. Circuit Breaking
1. Swagger

Each of these steps builds on the one before it, so it is best to
do them in the order shown. However, each lab is designed to stand
alone.

### Setup

The objective of this lab is to demonstrate a basic project with only a minimal, basic
controller that mirrors the _ValuesController_ in a brand new .NET API project.

When complete, you will have a simple REST API with the endpoint `/api/values` that
you can view both locally and deployed to Pivotal Cloud Foundry (PCF)!

### Connectors

Continuing from "Setup", the objective of this lab is to build on the Setup lab by adding a model,
repository, service, and new controller for handling the new _Bicycle_ class.

When complete, you will have a simple REST API with bicycle endpoints at
`/api/bicycle` to expose Create, Read, Update, and Delete (CRUD) operations.

In the lab, you will deploy the API to PCF.

### Configuration

Continuing from "Connectors", the objective of this lab is to demonstrate how to
use the a configuration service in PCF. Using a configuration service enables you to
externalize your configuration, which is one of the factors in
[The Twelve-Factor App](https://12factor.net/).

When complete, you will have a REST API that uses configurable values loaded from
an external configuration source.

### Management

Continuing from "Configuration", the objective of this lab is to demonstrate how to
enable the management endpoints in your Spring Boot-based REST API.

When complete, you will call new endpoints in your REST API to view information
about the state of your application, such as health and information endpoints.

### Circuit Breaking

Continuing from "Management", the objective of this lab is to demonstrate how to
use [Netflix's Hystrix library](https://github.com/Netflix/Hystrix) to provide a
"[CircuitBreaker](https://martinfowler.com/bliki/CircuitBreaker.html)" for
elegantly handling errors in a cloud-native application.

When complete, you will be able to see the circuit breaker in action to handle
errors gracefully.

### Swagger

Continuing from "Circuit Breaking", the objective of this final lab is to
demonstrate how a finished product should look with JUnit tests, unit test
coverage reporting, Checkstyle reporting, and generated Swagger documentation.

When complete, you will have an cloud-native API using best practices.

## Building the labs

TODO

## Running the labs

Each of the labs are designed to run locally as well as in
[Pivotal Cloud Foundry (PCF)](https://docs.pivotal.io/pivotalcf/2-6/concepts/overview.html).
The details of running each step are provided in the `README`
document in each of the directories. For example, to learn how to
build and test the very first example, see [Setup's README](./01-setup/README.md).

## Testing the API

The APIs can be tested using any utility that is used for testing REST APIs, such
as [`curl`](https://curl.haxx.se/), [`wget`](https://www.gnu.org/software/wget/),
or [`http`](https://httpie.org/). Further details can be found in the `README` documents
located in each lab's directory.

## Deploying the labs

The code in the labs can be executed both locally and in PCF. See the `README`
documents located in each lab's directory for more information about running each
lab.

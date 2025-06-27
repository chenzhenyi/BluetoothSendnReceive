# BluetoothSendnReceive

Bluetooth File Transfer Service ID Tester
This project provides a framework for testing Bluetooth file transfer functionality based on the Bluetooth service ID (UUID). It is designed to help developers verify that file transfers are correctly routed and handled according to the targeted Bluetooth service, ensuring compliance with Bluetooth profiles and robust interoperability across devices.

Overview
Bluetooth file transfer typically relies on specific service identifiers (UUIDs) to distinguish between different services and ensure that data is sent to the correct endpoint. For instance, the Object Push Profile (OPP) and File Transfer Profile (FTP) each have their own assigned UUIDs, which are critical for service discovery and connection establishment.

This project enables you to:

Discover Bluetooth devices and enumerate their available services by UUID.
Initiate file transfers to a selected device using a specified service UUID.
Verify that the transfer occurs only when the correct service ID is used.

Features
Service Discovery: Scan for nearby Bluetooth devices and list their supported services by UUID.
File Transfer: Send files to a target device using a specified Bluetooth service ID.
Validation: Confirm successful transfers and handle failures when the service ID does not match.
Logging: Detailed logs of connection attempts, service discovery, and transfer outcomes.

How It Works
Device Discovery: The application scans for nearby Bluetooth devices.
Service Enumeration: For each device, it lists available services using their UUIDs.
Service Selection: The user selects a target device and specifies the service ID (UUID) for file transfer.
Connection Establishment: The app attempts to connect to the service using the selected UUID.
File Transfer: If the connection is successful, the app transfers the selected file and logs the result.

Bluetooth Service IDs
Bluetooth services are identified by 128-bit UUIDs (also represented as 16- or 32-bit values in some APIs). For file transfer, common service IDs include:
Object Push Profile (OPP): Used for simple file transfers (e.g., vCards, images).
File Transfer Profile (FTP): Used for browsing and transferring files/folders.
Refer to the [Bluetooth Assigned Numbers documentation] for standard service UUIDs.

Sample Usage
Run BluetoothSendnReceive.exe on the machine receving the file
Run BluetoothClient.exe on the machine sending the file 

Requirements
Bluetooth-enabled device (Windows PC)
Target device must support the intended Bluetooth profile/service

Installation
Clone this repository.
Open the project
Build and deploy to your test device.

Testing Workflow
Start the application and scan for devices.
Select a device and specify the service UUID.
Choose a file to transfer.
Initiate the transfer and observe the log for success/failure.
Repeat with different service IDs to test service-based access control.

References
Bluetooth Assigned Numbers and UUIDs
Bluetooth File Transfer Profile specifications

Note: This project is intended for development and testing purposes. For production use, ensure compliance with platform security guidelines and Bluetooth SIG specifications.

# Car Insurance Sales Telegram Bot

## Overview

This bot assists users in purchasing car insurance by processing user-submitted documents, interacting through AI-driven communications, and confirming transaction details. 

## Features

- **Document Submission**: Users can submit photos of their driver license and vehicle identification document.
- **Data Extraction**: Extracts data from submitted documents using the Mindee API.
- **Data Confirmation**: Users confirm the extracted data.
- **Price Quotation**: Fixed price quotation for insurance.
- **Policy Issuance**: Generates and sends a dummy insurance policy document.

## Requirements

- [.NET Core 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- Telegram Bot API
- Mindee API
- OpenAI API

## Setup Instructions

1. **Clone the Repository**:
    git clone https://github.com/Valieiev/Diceus-test-task
2. **Install Dependencies**:
    dotnet restore
3. **Set API token**
    In the Program.cs file, replace the token for OpenAIAPI(“your-api-key”)
4. **Run the Bot**:
    dotnet run

## Bot Workflow

1. **User**: `/start`
2. **Bot**: "Welcome! Please send a photo of your driver license."
3. **User**: [Sends driver license photo]
4. **Bot**: "Driver license photo received. Now, please send a photo of your vehicle identification document."
5. **User**: [Sends vehicle photo]
6. **Bot**: "Vehicle identification document received. "Driver license: [Extracted Driver License Data] Vehicle Data: [Extracted Vehicle Data]  Please confirm if this data is correct. (yes/no)"
7. **User**: "Yes"
8. **Bot**: "The fixed price for the insurance is 100 USD. Do you agree?"
9. **User**: "Yes"
10. **Bot**: "Here is your dummy insurance policy document: [Generated Policy]"

By following this workflow, the bot collects both the driver license and vehicle identification document photos, processes them, confirms the extracted data with the user, and then provides the insurance quotation and policy document.

## State Management

The bot uses a state machine to handle different stages of the user interaction. Here's a detailed description of how the bot`s states change:

1. **Initial State**: 
    - When the user sends `/start`, the bot sets the state to `WaitingForDriverLicenseState`.
    - The bot prompts the user to send a photo of their driver license.

2. **WaitingForDriverLicenseState**:
    - The bot waits for the user to send a Driver License.
    - Upon receiving the photo, the bot processes it using the Mindee API.
    - After processing, the bot sets the state to `WaitingForVehicleCertificatePhotoState`.
    - The bot prompts the user to send a photo of their vehicle identification document.

3. **WaitingForVehicleCertificatePhotoState**:
    - The bot waits for the user to send a Vehicle Certificate photo.
    - Upon receiving the photo, the bot processes it using the Mindee API.
    - After processing, the bot sets the state to `WaitingForConfirmationState`.
    - The bot presents the extracted data to the user for confirmation.

4. **WaitingForConfirmationState**:
    - The bot waits for the user to confirm the extracted data.
    - If the user confirms the data, the bot sets the state to `WaitingForAgreementState`.
    - If the user requests a retake, the bot returns to `WaitingForDriverLicense` state.

5. **WaitingForAgreementState**:
    - The bot informs the user about the fixed price for the insurance.
    - If the user agrees to the price, generates a car insurance document
    - After processing, the bot sets the state to `WaitingForDriverLicenseState`
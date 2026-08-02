# TunNetCom.SharedKernel.Helpers

Shared helper utilities for TunNetCom applications.

## Features

- `DecimalHelper` - decimal rounding helpers for amounts and percentages.

## Installation

```
dotnet add package TunNetCom.SharedKernel.Helpers
```

## Usage

```csharp
using TunNetCom.SharedKernel.Helpers;

decimal amount = DecimalHelper.RoundAmount(1.2345m);       // 1.235
decimal percentage = DecimalHelper.RoundPercentage(19.995m); // 20.00
```

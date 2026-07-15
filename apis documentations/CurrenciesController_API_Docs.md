# Currencies API Documentation

This document describes the currency management endpoints provided by the `CurrenciesController`. These endpoints allow fetching available currencies.

---

## 1. Get All Currencies

Retrieves a list of all currencies in the system.

- **URL:** `/api/Currencies`
- **Method:** `GET`
- **Authentication:** None required (Publicly accessible)

### Responses

**Success (200 OK):**
```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": [
    {
      "id": 1,
      "name": "ريال يمني"
    },
    {
      "id": 2,
      "name": "ريال سعودي"
    }
  ]
}
```

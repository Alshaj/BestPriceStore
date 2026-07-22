# Retrieve All Orders (Admin) API Documentation

This document describes the updated `GET /api/admin/orders` endpoint for retrieving all orders in the system, supporting search by store name or order number, and status filtering.

---

## Endpoint Details

- **URL:** `/api/admin/orders`
- **Method:** `GET`
- **Authentication:** Required (Bearer Token, Role: `Admin`)
- **Query Parameters:**
  - `search` (optional, string): Search term matching:
    - **Order Number** (if the value is an integer, matching the order ID exactly).
    - **Store Name** (performs a case-insensitive partial match on the client's store name).
  - `orderStatusId` (optional, integer): Filters the retrieved list by status ID.
    - `1` = Pending (قيد المراجعة)
    - `2` = Processing (قيد المعالجة)
    - `3` = Shipped (تم الشحن)
    - `4` = Delivered (تم التوصيل)
    - `5` = Cancelled (ملغى)
    - **Note:** If omitted, no status filter is applied.

---

## Response Schema (200 OK)

Returns an API envelope containing a list of order summary objects.

```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": [
    {
      "id": 27,
      "orderStatusId": 1,
      "totalAmountYer": 50.0,
      "totalAmountSar": 222.0,
      "createdAt": "2026-07-22T15:51:00",
      "userId": 18,
      "storeName": "Mukalla Store1"
    }
  ]
}
```

### Field Reference (data item)

| Field | Type | Description |
|-------|------|-------------|
| `id` | `integer` | Unique ID (Order Number) of the order. |
| `orderStatusId` | `integer` | Current status ID of the order. |
| `totalAmountYer` | `double` | Total amount in Yemeni Rials (YER). |
| `totalAmountSar` | `double` | Total amount in Saudi Rials (SAR). |
| `createdAt` | `string` | Order creation date and time. |
| `userId` | `integer` | Unique ID of the user who placed the order. |
| `storeName` | `string` | The store name of the user who placed the order. |

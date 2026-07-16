# Orders & Order Statuses API Documentation

This document describes the endpoints for creating, listing, viewing details, and canceling orders, as well as resolving their statuses.

---

## 1. Get Order Statuses

Returns all available order status lookup definitions (ID and name pairs). The frontend uses this to resolve integer `OrderStatusId` values to their localized/display names.

- **URL:** `/api/OrderStatuses`
- **Method:** `GET`
- **Authentication:** None (Public)

### Response (200 OK)

```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": [
    {
      "id": 1,
      "name": "Pending"
    },
    {
      "id": 2,
      "name": "Processing"
    },
    {
      "id": 3,
      "name": "Shipped"
    },
    {
      "id": 4,
      "name": "Delivered"
    },
    {
      "id": 5,
      "name": "Cancelled"
    }
  ]
}
```

---

## 2. Place Order

Creates a new order, validates stock, reduces variation stock levels, and records transaction details.

- **URL:** `/api/Orders`
- **Method:** `POST`
- **Authentication:** Required (Bearer Token)

### Request Body (CreateOrderRequestDTO)

```json
{
  "items": [
    {
      "productImageId": 1,
      "quantity": 1
    },
    {
      "productImageId": 5,
      "quantity": 2
    }
  ]
}
```

#### Field Specifications

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| `items` | `array` | Yes | Min 1 item | List of items to purchase. |
| `items[].productImageId` | `integer` | Yes | - | The unique ID of the product image variation being ordered. |
| `items[].quantity` | `integer` | Yes | Must be >= 1 | The quantity requested for this variation. |

### Responses

#### Success (201 Created)
```json
{
  "statusCode": 201,
  "success": true,
  "errors": [],
  "data": {
    "id": 8,
    "userId": 18,
    "orderStatusId": 1,
    "totalAmountYer": 1200.0,
    "totalAmountSar": 250.0,
    "createdAt": "2026-07-16T18:39:05.0450581Z",
    "items": [
      {
        "productId": 1,
        "productName": "وساده مريحه",
        "productImageId": 1,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/a7242460-ea0f-4abd-89e0-58c00e7cb1bc.jpeg",
        "quantity": 1,
        "unitPrice": 1200.0,
        "totalAmount": 1200.0,
        "currencyId": 1
      },
      {
        "productId": 2,
        "productName": "عمر",
        "productImageId": 5,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/1856aad6-a29d-46a3-91ce-5e0bb919d7b8.jpeg",
        "quantity": 1,
        "unitPrice": 250.0,
        "totalAmount": 250.0,
        "currencyId": 2
      }
    ]
  }
}
```

#### Bad Request - Insufficient Stock (400 Bad Request)
```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": [
    "Insufficient stock for product 'عمر' (ID: 2, Variation ID: 5). Available: 1, Requested: 5."
  ]
}
```

---

## 3. List My Orders

Retrieves a summary list of all orders belonging to the logged-in user, ordered by creation date descending.

- **URL:** `/api/Orders`
- **Method:** `GET`
- **Authentication:** Required (Bearer Token)

### Response (200 OK)

```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": [
    {
      "id": 8,
      "orderStatusId": 1,
      "totalAmountYer": 1200.0,
      "totalAmountSar": 250.0,
      "createdAt": "2026-07-16T18:39:05.0450581"
    }
  ]
}
```

---

## 4. Get Order Details

Retrieves complete detail information for a specific order. Useful when the user clicks on an order to see its product list.

- **URL:** `/api/Orders/{id}`
- **Method:** `GET`
- **Authentication:** Required (Bearer Token)

### Response (200 OK)

```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": {
    "id": 8,
    "userId": 18,
    "orderStatusId": 1,
    "totalAmountYer": 1200.0,
    "totalAmountSar": 250.0,
    "createdAt": "2026-07-16T18:39:05.0450581",
    "items": [
      {
        "productId": 1,
        "productName": "وساده مريحه",
        "productImageId": 1,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/a7242460-ea0f-4abd-89e0-58c00e7cb1bc.jpeg",
        "quantity": 1,
        "unitPrice": 1200.0,
        "totalAmount": 1200.0,
        "currencyId": 1
      },
      {
        "productId": 2,
        "productName": "عمر",
        "productImageId": 5,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/1856aad6-a29d-46a3-91ce-5e0bb919d7b8.jpeg",
        "quantity": 1,
        "unitPrice": 250.0,
        "totalAmount": 250.0,
        "currencyId": 2
      }
    ]
  }
}
```

#### Error - Order Not Found / Unauthorized Ownership (404 Not Found)
Returned if the order ID doesn't exist or is owned by a different user.
```json
{
  "statusCode": 404,
  "success": false,
  "data": null,
  "errors": [
    "Order not found."
  ]
}
```

---

## 5. Cancel Order

Cancels an order. This operation is only permitted if the order's current status is **`Pending` (`orderStatusId: 1`)**. Canceling an order releases and restores the reserved product image stock back to inventory.

- **URL:** `/api/Orders/{id}/cancel`
- **Method:** `PUT`
- **Authentication:** Required (Bearer Token)

### Response (200 OK)
Returns the updated order summary with status changed to **`Cancelled` (`orderStatusId: 5`)**.

```json
{
  "statusCode": 200,
  "success": true,
  "errors": [],
  "data": {
    "id": 8,
    "userId": 18,
    "orderStatusId": 5,
    "totalAmountYer": 1200.0,
    "totalAmountSar": 250.0,
    "createdAt": "2026-07-16T18:39:05.0450581",
    "items": [
      {
        "productId": 1,
        "productName": "وساده مريحه",
        "productImageId": 1,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/a7242460-ea0f-4abd-89e0-58c00e7cb1bc.jpeg",
        "quantity": 1,
        "unitPrice": 1200.0,
        "totalAmount": 1200.0,
        "currencyId": 1
      }
    ]
  }
}
```

#### Error - Order Already Processed / Cannot Cancel (400 Bad Request)
Returned if the order is already in a non-cancelable state (e.g., Processing, Shipped, Cancelled).
```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": [
    "Only orders with 'Pending' status can be cancelled."
  ]
}
```

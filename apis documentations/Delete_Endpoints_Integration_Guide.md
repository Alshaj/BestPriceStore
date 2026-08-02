# Integration Guide: Representative Hard Delete & Admin Order Soft Delete APIs

This document describes the specifications and integration details for two new endpoints:
1. **Hard Delete Representative** (`DELETE /api/users/representatives/{id}`)
2. **Soft Delete Order** (`DELETE /api/admin/orders/{id}`)

---

## 1. Hard Delete Representative API

Permanently deletes a representative user and all associated data from the system.

### Endpoint Details
* **Method:** `DELETE`
* **URL:** `/api/users/representatives/{id}`
* **Authentication:** Required (`Bearer <Admin_JWT_Token>`, Role: `Admin`)
* **URL Path Parameters:**
  * `id` (integer, required): ID of the representative to be deleted.

### Backend Behavior
* Checks if the target user exists. If not, returns `404 Not Found`.
* Checks if the target user belongs to the `Representative` role. If not (e.g. attempting to delete an Admin), returns `400 Bad Request`.
* Executes inside a database transaction:
  * Permanently deletes representative's `Cart` and `CartItems`.
  * Permanently deletes representative's `Orders` and `OrderProducts`.
  * Permanently deletes user account (`AspNetUsers`) along with all Identity roles/claims/tokens.

### Response Examples

#### 200 OK (Success)
```json
{
  "statusCode": 200,
  "success": true,
  "data": {
    "message": "Representative and all associated data have been permanently deleted."
  },
  "errors": []
}
```

#### 404 Not Found
```json
{
  "statusCode": 404,
  "success": false,
  "data": null,
  "errors": [
    "User not found."
  ]
}
```

#### 400 Bad Request
```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": [
    "The specified user is not a representative."
  ]
}
```

#### 401 Unauthorized / 403 Forbidden
Returned when the Authorization header is missing, invalid, or the user is not an Admin.

### Frontend Integration Example (TypeScript / Axios)
```typescript
import axios from 'axios';

export const deleteRepresentative = async (repId: number, token: string) => {
  const response = await axios.delete(`/api/users/representatives/${repId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return response.data;
};
```

---

## 2. Soft Delete Order API (Admin)

Marks an order as soft-deleted (`IsDeleted = true`).

### Endpoint Details
* **Method:** `DELETE`
* **URL:** `/api/admin/orders/{id}`
* **Authentication:** Required (`Bearer <Admin_JWT_Token>`, Role: `Admin`)
* **URL Path Parameters:**
  * `id` (integer, required): ID of the order to soft-delete.

### Backend Behavior & Rules
* Sets `IsDeleted = true` for the target order record.
* **Admin Visibility:**
  * Hidden from Admin order list (`GET /api/admin/orders`).
  * Hidden from Admin order details (`GET /api/admin/orders/{id}`).
  * Cannot update status or edit items for soft-deleted orders.
  * Excluded from Admin Dashboard statistics (`GET /api/admin/dashboard/stats`).
* **Representative Visibility:**
  * **The representative who placed the order can STILL view it** in their order history (`GET /api/orders` & `GET /api/orders/{id}`).

### Response Examples

#### 200 OK (Success)
```json
{
  "statusCode": 200,
  "success": true,
  "data": {
    "message": "Order has been successfully deleted."
  },
  "errors": []
}
```

#### 404 Not Found
Returned if the order does not exist or has already been soft-deleted.
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

#### 401 Unauthorized / 403 Forbidden
Returned when the Authorization header is missing, invalid, or the user is not an Admin.

### Frontend Integration Example (TypeScript / Axios)
```typescript
import axios from 'axios';

export const softDeleteOrder = async (orderId: number, token: string) => {
  const response = await axios.delete(`/api/admin/orders/${orderId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return response.data;
};
```

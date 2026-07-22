# Frontend Integration Guide: Server-Side Order Filtering

To improve performance and ensure data consistency, the filtering of orders has been moved from the frontend to the backend. The frontend should no longer retrieve all orders and filter them locally. Instead, it must pass the selected `OrderStatusId` as a query parameter directly to the API endpoint.

## API Changes

### Endpoint
* **URL**: `/api/Orders`
* **Method**: `GET`
* **Headers**: `Authorization: Bearer <token>`
* **Query Parameters**:
  * `orderStatusId` (optional, integer): The ID of the status to filter by.
    * If omitted or null, the API returns **all** orders for the logged-in user.
    * If provided, the API returns only orders matching that specific status ID.

### Query Parameters Reference (OrderStatusId)
* `1` = Pending (قيد المراجعة)
* `2` = Processing (قيد المعالجة)
* `3` = Shipped (تم الشحن)
* `4` = Delivered (تم التوصيل)
* `5` = Cancelled (ملغى)

---

## Action Items for Frontend Integration

### 1. Update API Request Function
Modify your API call function to accept the optional `orderStatusId`.

**Example (TypeScript / JavaScript):**
```typescript
// BEFORE:
// const getMyOrders = () => axios.get('/api/Orders');

// AFTER:
const getMyOrders = (orderStatusId?: number) => {
  const url = orderStatusId ? `/api/Orders?orderStatusId=${orderStatusId}` : '/api/Orders';
  return axios.get(url);
};
```

### 2. Trigger API Fetch on Filter Tab Switch
Instead of filtering the local array when a user taps a status tab (e.g., "تم التوصيل" or "ملغى"), trigger a new API call using the corresponding `OrderStatusId`.

**Example (React / Flutter Flow logic):**
```typescript
const handleTabChange = async (statusId: number | null) => {
  setSelectedTab(statusId);
  setIsLoading(true);
  try {
    // Call the API with the selected status ID (or null/undefined for "All")
    const response = await getMyOrders(statusId);
    setOrders(response.data.data); // Update state directly with backend response
  } catch (error) {
    console.error("Failed to fetch filtered orders:", error);
  } finally {
    setIsLoading(false);
  }
};
```

### 3. Remove Client-Side Filtering Logic
Ensure that any local array `.filter(...)` logic for `OrderStatusId` is removed from your UI component render methods, as the API response will already be pre-filtered by the backend.

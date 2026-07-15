# Create Product API Documentation

This document describes the product creation endpoint provided by the `ProductsController` and explains the integration flow for the **Add Product** screen.

---

## Screen Integration Flow (Add Product Screen)

1. **Fetch Currencies**: Call `GET /api/Currencies` to fetch options for the currency radio buttons (documented in [CurrenciesController_API_Docs.md](file:///c:/Users/abdurahman/source/repos/BestPriceStore/BestPriceStore/apis%20documentations/CurrenciesController_API_Docs.md)).
2. **Fetch Categories**: Call `GET /api/Categories` to fetch options for the category combo box (documented in [CategoriesController_API_Docs.md](file:///c:/Users/abdurahman/source/repos/BestPriceStore/BestPriceStore/apis%20documentations/CategoriesController_API_Docs.md)).
3. **Upload Image(s)**: Call `POST /api/Images/upload` to upload each selected image and get back its public URL (documented in [ImagesController_API_Docs.md](file:///c:/Users/abdurahman/source/repos/BestPriceStore/BestPriceStore/apis%20documentations/ImagesController_API_Docs.md)).
4. **Save Product**: Call `POST /api/products` with the details and image URLs (detailed below).

---

## 1. Create Product

Creates a new product in the system and stores the uploaded image URLs with their stock quantities.

- **URL:** `/api/products`
- **Method:** `POST`
- **Authentication:** Required (Bearer Token)
- **Role Required:** Admin

### Request Headers
| Header | Value |
|--------|-------|
| `Authorization` | `Bearer {admin_jwt_token}` |
| `Content-Type` | `application/json` |

### Request Body
```json
{
  "name": "كابل شاحن سريع",
  "description": "كابل شاحن تايب سي عالي الجودة يدعم الشحن السريع",
  "price": 45.0,
  "currencyId": 2,
  "categoryId": 1,
  "images": [
    {
      "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/3a79d363-2287-43f1-b9cd-0e3d937000af.png",
      "quantityInStock": 150
    },
    {
      "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/da037300-8459-467f-94ad-73000afdae62.png",
      "quantityInStock": 75
    }
  ]
}
```

### Request Parameters

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| `name` | `string` | Yes | Max 255 chars | The name of the product. |
| `description` | `string` | No | - | Optional product description. |
| `price` | `double` | Yes | Must be > 0 | Price of the product. |
| `currencyId` | `integer` | Yes | Must match valid Currency ID | The ID of the currency. |
| `categoryId` | `integer` | Yes | Must match valid Category ID | The ID of the category. |
| `images` | `array` | No | - | List of product image objects. |
| `images[].imageUrl` | `string` | Yes | Valid URL | Public R2 URL from image upload. |
| `images[].quantityInStock` | `integer` | Yes | Must be >= 0 | Stock quantity for this variation. |
| `images[].isPrimary` | `boolean` | No | Defaults to `false` | Sets image as primary. If omitted, the first image automatically becomes primary. |

### Responses

**Success (201 Created):**
```json
{
  "statusCode": 201,
  "success": true,
  "errors": [],
  "data": {
    "id": 12,
    "name": "كابل شاحن سريع",
    "description": "كابل شاحن تايب سي عالي الجودة يدعم الشحن السريع",
    "price": 45.0,
    "currencyId": 2,
    "currencyName": "ريال سعودي",
    "categoryId": 1,
    "categoryName": "إلكترونيات",
    "createdAt": "2026-07-15T13:48:00Z",
    "updatedAt": "2026-07-15T13:48:00Z",
    "isActive": true,
    "images": [
      {
        "id": 15,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/3a79d363-2287-43f1-b9cd-0e3d937000af.png",
        "quantityInStock": 150,
        "isPrimary": true
      },
      {
        "id": 16,
        "imageUrl": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/da037300-8459-467f-94ad-73000afdae62.png",
        "quantityInStock": 75,
        "isPrimary": false
      }
    ]
  }
}
```

**Bad Request (400 Bad Request):**
*Returned if the validation fails (e.g. invalid Price, Category ID, or Currency ID).*
```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": [
    "Category with ID 999 does not exist."
  ]
}
```



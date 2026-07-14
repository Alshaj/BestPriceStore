# Images Controller API Documentation

This controller handles generic image uploads using Cloudflare R2 bucket storage.

## Endpoints

### 1. Upload Image

Uploads a single image to Cloudflare R2 and returns a clickable public URL.

- **URL:** `/api/Images/upload`
- **Method:** `POST`
- **Authentication:** Required (Bearer Token)
- **Role Required:** Admin

#### Request Format
- **Content-Type:** `multipart/form-data`

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `file` | `IFormFile` (file) | Yes | The image file to be uploaded. |

#### Request Headers
| Header | Value |
|--------|-------|
| `Authorization` | `Bearer {admin_jwt_token}` |

#### Responses

**Success (200 OK):**
```json
{
  "statusCode": 200,
  "success": true,
  "errors": null,
  "data": {
    "message": "Image uploaded successfully",
    "url": "https://pub-4e485becda324bc392c5253fecb937cd.r2.dev/3a79d363-2287-43f1-b9cd-0e3d937000af.png"
  }
}
```

**Bad Request (400 Bad Request):**
*If the file is not provided or empty.*
```json
{
  "statusCode": 400,
  "success": false,
  "data": "File cannot be empty",
  "errors": null
}
```

**Unauthorized (401 / 403):**
*If the token is missing, invalid, or the user is not an Admin.*

# HTTP Method Guidelines

A comprehensive guide on when and how to use each HTTP method in RESTful API design.

## Overview

HTTP methods (also known as verbs) define the action to be performed on a resource. Choosing the right HTTP method is crucial for building RESTful APIs that are intuitive, consistent, and follow industry standards.

## HTTP Methods

### GET

#### Purpose
Retrieve representations of resources without modifying server state.

#### Characteristics
- **Idempotent**: Yes (multiple identical requests should have the same effect)
- **Safe**: Yes (should not modify server state)
- **Cacheable**: Yes

#### When to Use
- Fetching a list of resources
- Fetching a single resource by ID
- Search operations
- Filtering data
- Pagination
- Read-only operations

#### Examples
```
GET /api/users                    # Get all users
GET /api/users/123                # Get user with ID 123
GET /api/users?status=active      # Get active users
GET /api/users?page=2&size=10     # Get paginated users
GET /api/search?q=john            # Search for users
```

#### Response Codes
- 200 OK - Successful response
- 404 Not Found - Resource doesn't exist
- 400 Bad Request - Invalid parameters

### POST

#### Purpose
Create new resources or submit data to be processed.

#### Characteristics
- **Idempotent**: No (multiple identical requests may create multiple resources)
- **Safe**: No (modifies server state)
- **Cacheable**: No

#### When to Use
- Creating new resources
- Submitting forms
- Login operations
- Complex operations that don't fit other methods
- Triggering background processes
- Sending data that will be processed

#### Examples
```
POST /api/users                   # Create a new user
POST /api/auth/login              # Authenticate user
POST /api/orders                  # Create a new order
POST /api/users/123/activate      # Activate a user (action endpoint)
```

#### Request Body
Always include a request body with the data to be processed.

#### Response Codes
- 201 Created - Resource successfully created
- 200 OK - Successful processing (for actions)
- 400 Bad Request - Invalid data
- 409 Conflict - Resource already exists

### PUT

#### Purpose
Update an entire resource or create a resource at a specific URI.

#### Characteristics
- **Idempotent**: Yes (multiple identical requests should have the same effect)
- **Safe**: No (modifies server state)
- **Cacheable**: No

#### When to Use
- Updating an entire resource
- Replacing a resource completely
- Creating a resource with a specific ID (upsert)
- Synchronizing resources

#### Examples
```
PUT /api/users/123                # Update user with ID 123
PUT /api/products/ABC123          # Update product with specific ID
```

#### Request Body
Include the complete representation of the resource.

#### Important Notes
- Should replace the entire resource
- If resource doesn't exist, it may create it (upsert)
- All fields should be provided, even if unchanged

#### Response Codes
- 200 OK - Resource updated successfully
- 201 Created - Resource created successfully
- 400 Bad Request - Invalid data
- 404 Not Found - Resource doesn't exist (if not supporting upsert)

### PATCH

#### Purpose
Partially update a resource.

#### Characteristics
- **Idempotent**: No (multiple identical requests may have different effects)
- **Safe**: No (modifies server state)
- **Cacheable**: No

#### When to Use
- Updating specific fields of a resource
- Making partial modifications
- When you don't want to send the entire resource representation

#### Examples
```
PATCH /api/users/123              # Update specific fields of user
PATCH /api/products/ABC123        # Update product price only
```

#### Request Body
Include only the fields that need to be updated.

#### Common Formats
1. **JSON Patch** (RFC 6902):
```json
[
  { "op": "replace", "path": "/name", "value": "New Name" },
  { "op": "add", "path": "/email", "value": "new@example.com" }
]
```

2. **Merge Patch** (RFC 7396):
```json
{
  "name": "New Name",
  "email": "new@example.com"
}
```

#### Response Codes
- 200 OK - Resource updated successfully
- 204 No Content - Update successful, no response body
- 400 Bad Request - Invalid patch document
- 404 Not Found - Resource doesn't exist

### DELETE

#### Purpose
Remove a resource.

#### Characteristics
- **Idempotent**: Yes (deleting a non-existent resource should still return success)
- **Safe**: No (modifies server state)
- **Cacheable**: No

#### When to Use
- Removing resources
- Canceling operations
- Cleaning up data

#### Examples
```
DELETE /api/users/123             # Delete user with ID 123
DELETE /api/orders/ABC123         # Cancel an order
```

#### Request Body
Typically no request body is included.

#### Response Codes
- 200 OK - Resource deleted successfully
- 204 No Content - Resource deleted successfully, no response body
- 404 Not Found - Resource doesn't exist

### HEAD

#### Purpose
Retrieve only the headers of a resource, not the body.

#### Characteristics
- **Idempotent**: Yes
- **Safe**: Yes
- **Cacheable**: Yes

#### When to Use
- Checking if a resource exists
- Getting metadata without downloading the full resource
- Checking resource size or last modified date

#### Examples
```
HEAD /api/users/123               # Check if user exists
HEAD /api/files/document.pdf      # Get file metadata
```

#### Response Codes
Same as GET but without response body.

### OPTIONS

#### Purpose
Describe the communication options for the target resource.

#### Characteristics
- **Idempotent**: Yes
- **Safe**: Yes
- **Cacheable**: Yes

#### When to Use
- Discovering allowed methods for a resource
- CORS preflight requests
- API exploration

#### Examples
```
OPTIONS /api/users                # Get allowed methods for users
OPTIONS /api/users/123            # Get allowed methods for specific user
```

## Best Practices

### 1. Follow REST Principles
- Use nouns, not verbs in URLs
- Use plural nouns for collections
- Use consistent naming conventions

### 2. Use Appropriate HTTP Status Codes
```
2xx - Success
  200 OK - General success
  201 Created - Resource created
  204 No Content - Successful operation with no response body

4xx - Client Errors
  400 Bad Request - Invalid request
  401 Unauthorized - Authentication required
  403 Forbidden - Access denied
  404 Not Found - Resource not found
  409 Conflict - Resource conflict

5xx - Server Errors
  500 Internal Server Error - General server error
  503 Service Unavailable - Service temporarily unavailable
```

### 3. Design Resource-Oriented URLs
```
✅ Good:
GET /api/users
POST /api/users
GET /api/users/123
PUT /api/users/123
DELETE /api/users/123

❌ Bad:
GET /api/getUsers
POST /api/createUser
GET /api/deleteUser?id=123
```

### 4. Use Query Parameters for Filtering and Sorting
```
GET /api/users?status=active&sort=name&order=asc&page=1&size=20
```

### 5. Handle Relationships Appropriately
```
GET /api/users/123/orders          # Get orders for user 123
POST /api/users/123/orders         # Create order for user 123
GET /api/users/123/orders/456      # Get specific order for user
```

### 6. Version Your API
```
GET /api/v1/users                  # Version 1
GET /api/v2/users                  # Version 2
```

## Common Patterns

### Collection vs Item Operations
```
GET    /api/users                  # Get all users (collection)
POST   /api/users                  # Create new user (collection)
GET    /api/users/123              # Get specific user (item)
PUT    /api/users/123              # Update specific user (item)
PATCH  /api/users/123              # Partially update user (item)
DELETE /api/users/123              # Delete specific user (item)
```

### Action Endpoints
For operations that don't fit CRUD:
```
POST /api/users/123/activate       # Activate user
POST /api/orders/456/cancel        # Cancel order
POST /api/files/import             # Import files
```

### Batch Operations
```
POST /api/users/batch              # Create multiple users
PATCH /api/users/batch             # Update multiple users
DELETE /api/users/batch            # Delete multiple users
```

## Security Considerations

1. **Authentication**: Use appropriate methods (JWT, OAuth, API Keys)
2. **Authorization**: Implement role-based or permission-based access control
3. **Rate Limiting**: Prevent abuse with request rate limiting
4. **Input Validation**: Always validate and sanitize input data
5. **HTTPS**: Always use HTTPS in production
6. **CORS**: Configure Cross-Origin Resource Sharing appropriately

## Error Handling

Use consistent error response formats:
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  }
}
```

## Caching Considerations

- **GET**: Can be cached
- **POST**: Should not be cached
- **PUT**: Cache should be invalidated
- **PATCH**: Cache should be invalidated
- **DELETE**: Cache should be invalidated

Use appropriate cache headers:
```
Cache-Control: public, max-age=3600
ETag: "abc123"
Last-Modified: Wed, 21 Oct 2023 07:28:00 GMT
```
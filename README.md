# HappyEat

> AI-powered diet and body management platform built with Vue.js, ASP.NET Core Web API, MSSQL, and Gemini API.

HappyEat is a health management platform designed to simplify daily diet tracking and body data management.

This repository focuses on the **Body Management** and **Food Diary** modules, combining body data visualization, nutrition tracking, and AI-assisted food recognition to provide a more intuitive and efficient health management experience.

---

## Project Overview

Traditional diet tracking often requires users to manually search for foods and enter nutrition information item by item.

HappyEat simplifies this process by integrating **Gemini AI image recognition**. Users can upload a meal photo, review the AI-generated nutrition analysis, provide additional context for re-analysis, manually adjust the results, and save the confirmed record.

The platform also connects body records and personal goals with diet tracking. Based on the user's latest body data, activity level, and goal, the system calculates daily recommended calorie intake and compares it with actual consumption.

---

## Key Features

### AI-Assisted Food Recognition

- Upload meal photos for AI-powered food recognition
- Analyze calories and macronutrients with Gemini API
- Prioritize integrated convenience-store nutrition data when applicable
- Provide additional context and request AI re-analysis
- Human-in-the-loop workflow before saving records
- Allow users to manually adjust AI-generated results
- Automatically clean up unused uploaded images when recognition or record creation is cancelled

### Food Diary

- Create, edit, delete, and view meal records
- Search food and drink nutrition data
- Automatically calculate nutrition based on quantity
- Track calories, carbohydrates, protein, and fat
- Filter meal records by meal type and date
- Display meal photos and nutrition details
- Summarize daily nutrition intake

### Body Management

- Record height, weight, body fat, muscle mass, waist size, and other body measurements
- Calculate BMI, BMR, and TDEE
- Visualize body measurement data
- Track recent body trends
- Manage weight goals and progress
- Calculate recommended daily calorie intake based on body data and user goals

---

## AI Recognition Workflow

```text
Upload Meal Photo
       ↓
Save FoodImage
       ↓
Gemini AI Analysis
       ↓
Display Recognition Result
       ↓
 ┌──────────────────────────┐
 │ Result needs correction? │
 └──────────────────────────┘
       ↓ Yes
Provide Additional Context
       ↓
Gemini Re-analysis
       ↓
Review Result
       ↓
Manual Adjustment
       ↓
User Confirmation
       ↓
Save FoodRecord
```

The AI result is **not saved directly**.

HappyEat uses a **Human-in-the-loop** approach, allowing users to verify and modify AI-generated nutrition information before creating the final food record.

---

## Tech Stack

### Frontend

- Vue 3
- JavaScript
- Tailwind CSS
- Axios
- Vue Router
- Chart.js / vue-chartjs

### Backend

- C#
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core (Database-First)
- RESTful API

### Database

- Microsoft SQL Server

### AI Integration

- Google Gemini API (Multi-modal Vision)
- Structured JSON Output (JSON Schema)
- Resilient Sequential Model Fallback & Timeout Control
---

## System Architecture

```text
Vue 3 Frontend
      ↓
API Service / Axios
      ↓
ASP.NET Core Web API
      ↓
Service Layer
      ↓
Entity Framework Core
      ↓
Microsoft SQL Server

           +

Meal Image
    ↓
FoodImage API
    ↓
Gemini Service
    ↓
Gemini API
    ↓
Nutrition Analysis
```

The frontend follows the following responsibility flow:

```text
View
 ↓
Component
 ↓
Composable
 ↓
API Service
 ↓
ASP.NET Core Web API
```

This separation keeps UI presentation, business interaction logic, and API communication independently maintainable.

---

## Database Design

Main entities used in the implemented modules include:

```text
Users
 ├── BodyRecords
 ├── UserGoals
 ├── FoodImages
 └── FoodRecords
          │
          └── FoodRecordItems
                 ├── Food
                 └── Drink
```

### Key Relationships

- One user can have multiple body records
- One user can have multiple food records
- One food record can contain multiple food record items
- A food record can optionally reference one uploaded image
- A food record item can reference either Food or Drink data
- AI-generated or custom items can exist without a Food or Drink foreign key

---

## Implementation Highlights

### Human-in-the-loop AI Design

AI recognition results are never directly stored as final records.

Users can:

1. Review the initial AI analysis
2. Provide additional information when the result is inaccurate
3. Request Gemini to analyze the same image again
4. Manually modify food names, quantities, and nutrition values
5. Save the record only after confirmation

This design reduces the impact of uncertain AI predictions while keeping the food logging experience efficient.

### Convenience-Store Nutrition Matching

During development, AI-generated nutrition values for packaged convenience-store foods were sometimes inaccurate.

To improve reliability, public nutrition information for selected convenience-store products is integrated into the backend analysis process.

When applicable, the system prioritizes these known nutrition values before generating the final analysis.

### Food Image Lifecycle Management

Meal images are stored independently from food records.

The system handles unused images to prevent orphaned database records and files:

```text
Upload Image
    ↓
AI Recognition
    │
    ├── Recognition Failed → Delete unused image
    │
    └── Recognition Success
              ↓
         User Confirmation
              │
              ├── Cancel → Delete unused image
              │
              └── Save → Keep image with FoodRecord
```

### Nutrition Calculation

Food and drink search results provide base nutrition values.

The frontend calculates nutrition dynamically based on quantity:

```text
Nutrition = Base Nutrition × Quantity
```

For example:

```text
Food:
1 unit = 100 g
1.5 units = 150 g

Drink:
1 unit = selected serving size
```

Meal-level calories and macronutrients are then calculated from all food record items.

### Recommended Daily Calories

Recommended calorie intake connects Body Management with Food Diary.

```text
User Profile
     +
Latest Body Record
     ↓
BMR
     ↓
TDEE
     +
Active User Goal
     ↓
Recommended Daily Calories
```

Current goal adjustment:

```text
Fat Loss    → TDEE - 300 kcal
Maintenance → TDEE
Muscle Gain → TDEE + 300 kcal
```

---

## Project Structure

```text
HappyEat-Portfolio/
│
├── frontend/
│   └── src/
│       ├── components/
│       │   ├── body/
│       │   └── food/
│       ├── composables/
│       ├── services/
│       ├── utils/
│       ├── views/
│       └── router/
│
└── backend/
    ├── Controllers/
    ├── DTOs/
    ├── Models/
    ├── Services/
    ├── Mapping/
    ├── Exceptions/
    └── Data/
```

> The actual folder names may differ depending on the local project structure.

---

## Getting Started

### Prerequisites

Make sure the following tools are installed:

- Node.js
- .NET SDK
- Microsoft SQL Server
- Visual Studio or Visual Studio Code

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Create a local `.env` file:

```env
VITE_API_BASE_URL=https://localhost:xxxx/api
VITE_SERVER_BASE_URL=https://localhost:xxxx
```

### Backend

Configure the local database connection string using development settings or User Secrets.

Configure the Gemini API key using .NET User Secrets:

```bash
dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY"
```

Then run:

```bash
dotnet restore
dotnet run
```

> API keys, local environment files, uploaded images, and development configuration files are excluded from version control.

---

## My Contribution

HappyEat was originally developed as a collaborative full-stack project.

My primary responsibilities focused on the **Body Management** and **Food Diary** modules, including:

- Body data management and visualization
- BMI / BMR / TDEE calculation
- User goal and recommended calorie calculation
- Food diary CRUD workflow
- Food and drink nutrition search
- Meal nutrition calculation
- Food image management
- Gemini AI food recognition integration
- AI re-analysis with user-provided context
- Human-in-the-loop confirmation workflow
- Convenience-store nutrition data integration
- Frontend and backend API integration
- Database relationship and business rule design

This repository presents and refines these features as an individual portfolio project.

---

## Future Improvements

- Authentication and user-specific authorization
- Automated testing
- Expanded nutrition database
- Improved AI recognition reliability
- Deployment and production environment configuration
- Responsive and accessibility improvements

---

## Author

**楊怡芳**

Full-stack developer transitioning from Industrial Engineering and semiconductor manufacturing, with experience in process analysis, production planning, cross-functional collaboration, and problem solving.

I aim to combine user-centered thinking with software development to build tools that solve practical problems.

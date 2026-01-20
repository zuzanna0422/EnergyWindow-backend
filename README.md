# EnergyWindow – Backend API

EnergyWindow Backend is a .NET Web API service responsible for fetching energy generation forecasts from an external Carbon Intensity API, processing the data, and exposing clean, optimized endpoints for the frontend dashboard.

The backend performs all business logic, including:

* Aggregating half-hour generation intervals into daily averages

* Calculating the percentage of clean (green) energy

* Determining the optimal charging window (1–6 hours) based on forecasted energy mix


## 🚀 Features

* External API integration (Carbon Intensity UK API)

* Clean architecture (Controllers → Services → Clients/Models → DTOs)

* Optimal charging window algorithm (sliding window)

* Daily energy mix aggregation

* Swagger / OpenAPI documentation

* Type-safe DTOs and null-safety handling

---
* **Frontend Repository:** [https://github.com/zuzanna0422/energywindow-frontend](https://github.com/zuzanna0422/energywindow-frontend)
* **Live Application:** [https://energywindow-frontend.onrender.com/](https://energywindow-frontend.onrender.com/)
* **Carbon Intensity API (UK):** [https://carbon-intensity.github.io](https://carbon-intensity.github.io)

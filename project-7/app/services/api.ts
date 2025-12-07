// services/api.ts
export const BASE_URL = "https://localhost:7277/api";

/* =========================
   NOTIFICATIONS
========================= */

export const fetchNotifications = async (userId: number) => {
  const res = await fetch(`${BASE_URL}/Notifications/${userId}`);
  if (!res.ok) throw new Error("Failed to load notifications");
  return await res.json();
};

export const sendNotification = async (dto: any) => {
  console.log("sendNotification called with DTO:", dto);
  const res = await fetch(`${BASE_URL}/Notifications/send`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  console.log("Response from sendNotification:", res);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || "Failed to send notification");
  }
  return await res.json();
};

/* =========================
   EVENTS
========================= */

export const fetchEvents = async (userId: number) => {
  const res = await fetch(`${BASE_URL}/Events/${userId}`);
  if (!res.ok) throw new Error("Failed to load events");
  return await res.json();
};

export const createEvent = async (dto: any) => {
  console.log("Creating event with DTO:", dto);
  const res = await fetch(`${BASE_URL}/Events`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || "Failed to create event");
  }
  return await res.json();
};

/* =========================
   TEMPLATES
========================= */

export const fetchTemplates = async () => {
  const res = await fetch(`${BASE_URL}/NotificationTemplates`);
  if (!res.ok) throw new Error("Failed to load templates");
  return await res.json();
};

export const createTemplate = async (dto: any) => {
  const res = await fetch(`${BASE_URL}/NotificationTemplates`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error("Failed to create template");
  return await res.json();
};

export const updateTemplate = async (id: number, dto: any) => {
  const res = await fetch(`${BASE_URL}/NotificationTemplates/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error("Failed to update template");
  return await res.json();
};

export const deleteTemplate = async (id: number) => {
  const res = await fetch(`${BASE_URL}/NotificationTemplates/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete template");
};

/* =========================
   SETTINGS
========================= */

export const fetchSettings = async () => {
  const res = await fetch(`${BASE_URL}/Settings`);
  if (!res.ok) throw new Error("Failed to load settings");
  return await res.json();
};

export const updateSettings = async (dto: any) => {
  const res = await fetch(`${BASE_URL}/Settings`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error("Failed to update settings");
  return await res.json();
};

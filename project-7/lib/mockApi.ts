export const fetchNotifications = async () => {
  try {
    const res = await fetch("/mock/notifications.json");
    if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
    return await res.json();
  } catch (err) {
    console.error("Failed to fetch notifications:", err);
    return []; // return empty array as fallback
  }
};

export const fetchEvents = async () => {
  try {
    const res = await fetch("/mock/events.json");
    if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
    return await res.json();
  } catch (err) {
    console.error("Failed to fetch events:", err);
    return [];
  }
};

export const fetchTemplates = async () => {
  try {
    const res = await fetch("/mock/templates.json");
    if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
    return await res.json();
  } catch (err) {
    console.error("Failed to fetch templates:", err);
    return [];
  }
};

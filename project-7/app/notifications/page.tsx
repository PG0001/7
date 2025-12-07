"use client";

import React, { useEffect, useState } from "react";
import { Container, Typography, Button, Box } from "@mui/material";
// Import Grid2 for newer syntax
import Grid from "@mui/material/Grid"; 
import NotificationCard from "@/Components/NotificationCard";
import { fetchNotifications } from "../services/api";
import Layout from "../ui/layout";

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<any[]>([]);
  const userId = 1;

  const loadNotifications = async () => {
    try {
      const data = await fetchNotifications(userId);
      setNotifications(data);
    } catch (err) {
      console.error(err);
      alert("Failed to load notifications");
    }
  };

  useEffect(() => {
    loadNotifications();

    // auto-refresh every 30s
    const id = setInterval(loadNotifications, 30000);
    return () => clearInterval(id);
  }, []);
console.log("Notifications:", notifications);
  return (
    <Layout>
      <Container sx={{ py: 4, backgroundColor: "#f3f4f6", minHeight: "100vh", borderRadius: 2 }}>
        {/* Header */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
          }}
        >
          <Typography variant="h5" sx={{ color: "#1f2937" }}>
            Notifications
          </Typography>
          <Button variant="contained" onClick={loadNotifications}>
            Refresh
          </Button>
        </Box>

      {/* Notification Grid */}
<Grid container spacing={2}>
  {notifications.length === 0 && (
    <Grid size={12}>
      <Box p={2} textAlign="center">
        No notifications
      </Box>
    </Grid>
  )}

  {notifications.map((n) => (
    <Grid key={n.NotificationId} size={12}>
      <NotificationCard notification={n} />
    </Grid>
  ))}
</Grid>

      </Container>
    </Layout>
  );
}

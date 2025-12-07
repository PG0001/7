// app/settings/page.tsx
"use client";
import React, { useEffect, useState } from "react";
import { Container, Typography, Paper, Box } from "@mui/material";
import SettingsForm from "@/Components/SettingsForm";
import { fetchSettings, updateSettings } from "../services/api";
import Layout from "../ui/layout";

export default function SettingsPage() {
  const [settings, setSettings] = useState<any | null>(null);

  const load = async () => {
    try {
      const data = await fetchSettings();
      setSettings(data);
    } catch (err) {
      console.error(err);
      alert("Failed to load settings");
    }
  };

  useEffect(() => {
    load();
  }, []);

  const save = async () => {
    try {
      await updateSettings(settings);
      alert("Saved");
    } catch (err: any) {
      console.error(err);
      alert(err.message ?? "Failed to save");
    }
  };

  return (
    <Layout>
      <Container maxWidth="md" sx={{ py: 5 }}>
        <Paper
          elevation={3}
          sx={{
            p: 4,
            borderRadius: 3,
            backgroundColor: "background.paper",
            boxShadow: "0px 4px 20px rgba(0,0,0,0.08)",
          }}
        >
          <Typography
            variant="h5"
            mb={3}
            sx={{ color: "primary.main", fontWeight: 600 }}
          >
            Admin Notification Settings
          </Typography>

          {settings ? (
            <Box>
              <SettingsForm
                settings={settings}
                onChange={setSettings}
                onSave={save}
              />
            </Box>
          ) : (
            <Typography variant="body1">Loading...</Typography>
          )}
        </Paper>
      </Container>
    </Layout>
  );
}

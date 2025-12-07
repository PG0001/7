"use client";
import React from "react";
import {
  Box,
  TextField,
  Switch,
  FormControlLabel,
  Button,
  Grid,
} from "@mui/material";

export default function SettingsForm({
  settings,
  onChange,
  onSave,
}: {
  settings: any;
  onChange: (value: any) => void;
  onSave: () => void;
}) {

  // Prevent undefined errors
  if (!settings) return <div>Loading...</div>;

  const updateField = (field: string, value: any) => {
    onChange({ ...settings, [field]: value });
  };

  return (
    <Box component="form" sx={{ mt: 2 }}>
      <Grid container spacing={3}>

        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControlLabel
            control={
              <Switch
                checked={settings.IsEmailEnabled ?? false}
                onChange={(e) =>
                  updateField("IsEmailEnabled", e.target.checked)
                }
              />
            }
            label="Enable Email"
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControlLabel
            control={
              <Switch
                checked={settings.IsSmsEnabled ?? false}
                onChange={(e) =>
                  updateField("IsSmsEnabled", e.target.checked)
                }
              />
            }
            label="Enable SMS"
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControlLabel
            control={
              <Switch
                checked={settings.Reminder24hrEnabled ?? false}
                onChange={(e) =>
                  updateField("Reminder24hrEnabled", e.target.checked)
                }
              />
            }
            label="24 Hour Reminder"
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControlLabel
            control={
              <Switch
                checked={settings.Reminder1hrEnabled ?? false}
                onChange={(e) =>
                  updateField("Reminder1hrEnabled", e.target.checked)
                }
              />
            }
            label="1 Hour Reminder"
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            fullWidth
            type="number"
            label="Reminder Hours Before Event"
            value={settings.ReminderHoursBeforeEvent ?? 0}
            onChange={(e) =>
              updateField("ReminderHoursBeforeEvent", Number(e.target.value))
            }
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            fullWidth
            type="number"
            label="Reminder Hours Before Event 2"
            value={settings.ReminderHoursBeforeEvent1 ?? 0}
            onChange={(e) =>
              updateField("ReminderHoursBeforeEvent1", Number(e.target.value))
            }
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            fullWidth
            type="number"
            label="Max Send Attempts"
            value={settings.MaxSendAttempts ?? 0}
            onChange={(e) =>
              updateField("MaxSendAttempts", Number(e.target.value))
            }
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <Button variant="contained" onClick={onSave} sx={{ mt: 2 }}>
            Save Settings
          </Button>
        </Grid>

      </Grid>
    </Box>
  );
}

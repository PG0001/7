"use client";
import React, { useEffect, useState } from "react";
import {
  Container,
  Typography,
  TextField,
  Button,
  MenuItem,
  Box,
} from "@mui/material";
import Layout from "../../ui/layout";
import { fetchTemplates, sendNotification } from "../../services/api";

interface Template {
  templateId: number;
  title: string;
  message: string;
  placeholders: Record<string, string>;
}

export default function SendNotificationPage() {
  const [templates, setTemplates] = useState<Template[]>([]);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    userId: 1,
    eventId: 0,
    channel: "Email", // Channel also used as Type
    templateId: 0,
    title: "",
    message: "",
    email: "",
    phone: "",
    placeholders: {} as Record<string, string>,
  });

  // Fetch templates on mount
  useEffect(() => {
    fetchTemplates()
      .then((data: any[]) => {
        // Normalize templates
        const normalized: Template[] = data.map((t: any) => {
          const placeholdersArray =
            t.Body?.match(/{{(.*?)}}/g)?.map((p: string) =>
              p.replace(/{{|}}/g, "")
            ) || [];
          const placeholdersObj = Object.fromEntries(
            placeholdersArray.map((p: string) => [p, ""])
          );
          return {
            templateId: t.TemplateId,
            title: t.TemplateName,
            message: t.Body,
            placeholders: placeholdersObj,
          };
        });
        console.log("Fetched templates:", normalized);
        setTemplates(normalized);
      })
      .catch((err) => console.error("Failed to fetch templates:", err));
  }, []);

  // Auto-fill form when template changes
  useEffect(() => {
    const selectedTemplate = templates.find(
      (t) => t.templateId === form.templateId
    );
    if (selectedTemplate) {
      setForm((prev) => ({
        ...prev,
        title: selectedTemplate.title,
        message: selectedTemplate.message,
        placeholders: selectedTemplate.placeholders,
      }));
    } else {
      // Clear placeholders if no template
      setForm((prev) => ({ ...prev, placeholders: {} }));
    }
  }, [form.templateId, templates]);

  const onSend = async () => {
    try {
      setLoading(true);

      const payload = {
        UserId: form.userId,
        EventId: form.eventId || null,
        Type: form.channel, // same as channel
        Title: form.title,
        Message: form.message,
        Email: form.email,
        Phone: form.phone,
        TemplateId: form.templateId || null,
        Placeholders: form.placeholders,
      };
      console.log("Sending notification with payload:", payload);

     const res = await sendNotification(payload);
console.log("Notification send response:", res);
      alert("Notification scheduled/sent successfully");
    } catch (err: any) {
      console.error(err);
      alert(err.message || "Send failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <Container
        sx={{
          py: 4,
          backgroundColor: "#f9fafb",
          minHeight: "100vh",
          borderRadius: 2,
        }}
      >
        <Typography variant="h5" mb={3} sx={{ color: "#111827" }}>
          Send Manual Notification
        </Typography>

        <Box
          component="form"
          sx={{ display: "grid", gap: 2, maxWidth: 720 }}
          onSubmit={(e) => {
            e.preventDefault();
            onSend();
          }}
        >
          {/* Basic Fields */}
          <TextField
            label="User ID"
            type="number"
            value={form.userId}
            onChange={(e) =>
              setForm({ ...form, userId: Number(e.target.value) })
            }
            required
          />

          <TextField
            label="Event ID (optional)"
            type="number"
            value={form.eventId}
            onChange={(e) =>
              setForm({ ...form, eventId: Number(e.target.value) })
            }
          />

          {/* Template Select */}
          <TextField
            select
            label="Template"
            value={form.templateId}
            onChange={(e) =>
              setForm({ ...form, templateId: Number(e.target.value) })
            }
          >
            <MenuItem value={0}>(none)</MenuItem>
            {templates.map((t) => (
              <MenuItem key={t.templateId} value={t.templateId}>
                {t.title}
              </MenuItem>
            ))}
          </TextField>

          {/* Channel / Type */}
          <TextField
            select
            label="Channel / Type"
            value={form.channel}
            onChange={(e) => setForm({ ...form, channel: e.target.value })}
          >
            <MenuItem value="Email">Email</MenuItem>
            <MenuItem value="SMS">SMS</MenuItem>
            <MenuItem value="Push">Push</MenuItem>
          </TextField>

          {/* Payload Fields */}
          <TextField
            label="Title"
            value={form.title}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
          />
          <TextField
            label="Message"
            multiline
            minRows={3}
            value={form.message}
            onChange={(e) => setForm({ ...form, message: e.target.value })}
          />

          {/* Conditional Email / Phone */}
          {(form.channel === "Email" || form.channel === "Push") && (
            <TextField
              label="Email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
            />
          )}
          {(form.channel === "SMS" || form.channel === "Push") && (
            <TextField
              label="Phone"
              value={form.phone}
              onChange={(e) => setForm({ ...form, phone: e.target.value })}
            />
          )}

          {/* Dynamic Placeholders */}
          {Object.keys(form.placeholders).map((key) => (
            <TextField
              key={key}
              label={key}
              value={form.placeholders[key]}
              onChange={(e) =>
                setForm({
                  ...form,
                  placeholders: {
                    ...form.placeholders,
                    [key]: e.target.value,
                  },
                })
              }
            />
          ))}

          <Button type="submit" variant="contained" disabled={loading}>
            {loading ? "Sending..." : "Send"}
          </Button>
        </Box>
      </Container>
    </Layout>
  );
}

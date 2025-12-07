"use client";

import { useEffect, useState } from "react";
import Layout from "../ui/layout";
import TemplateForm from "@/Components/TemplateForm";

import {
  Box,
  Container,
  Paper,
  Typography,
  Divider,
  Button,
  IconButton,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";

import { fetchTemplates, createTemplate, updateTemplate, deleteTemplate } from "../services/api";

export default function TemplatesPage() {
  const [templates, setTemplates] = useState<any[]>([]);
  const [editing, setEditing] = useState<any | null>(null);
  const [creating, setCreating] = useState(false);

 const loadTemplates = async () => {
  try {
    const data = await fetchTemplates();
    console.log("Raw fetched templates:", data);

    const normalized = data.map((t: any) => {
      // Extract placeholders from the Body
      const placeholdersArray = t.Body?.match(/{{(.*?)}}/g)?.map((p:string) => p.replace(/{{|}}/g, "")) || [];
      const placeholdersObj = Object.fromEntries(placeholdersArray.map((p:string) => [p, ""]));

      return {
        templateId: t.TemplateId,
        title: t.TemplateName,
        message: t.Body,
        placeholders: placeholdersObj,
      };
    });

    console.log("Fetched templates:", normalized);
    setTemplates(normalized);
  } catch (err) {
    console.error(err);
    alert("Failed to load templates");
  }
};


  useEffect(() => {
    loadTemplates();
  }, []);

  const handleCreate = async (t: any) => {
    await createTemplate({ TemplateName: t.title, Body: t.message, Placeholders: t.placeholders });
    setCreating(false);
    await loadTemplates();
  };

  const handleUpdate = async (id: number, t: any) => {
    await updateTemplate(id, { TemplateName: t.title, Body: t.message, Placeholders: t.placeholders });
    setEditing(null);
    await loadTemplates();
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Delete this template?")) return;
    await deleteTemplate(id);
    await loadTemplates();
  };

  return (
    <Layout>
      <Container maxWidth="md" sx={{ py: 4 }}>
        {/* Header */}
        <Box sx={{ display: "flex", justifyContent: "space-between", mb: 3 }}>
          <Typography variant="h4" color="Green" fontWeight={700}>Notification Templates</Typography>
          <Button variant="contained" onClick={() => setCreating(true)}>New Template</Button>
        </Box>

        {/* Create Form */}
        {creating && (
          <Paper sx={{ p: 3, mb: 4, borderRadius: 3, backgroundColor: "background.paper" }}>
            <TemplateForm onTemplateCreated={handleCreate} onCancel={() => setCreating(false)} />
          </Paper>
        )}

        {/* Templates List */}
        <Box display="flex" flexDirection="column" gap={2}>
          {templates.length === 0 && (
            <Paper sx={{ p: 3, textAlign: "center", borderRadius: 3, backgroundColor: "grey.50" }}>
              No templates found
            </Paper>
          )}

          {templates.map((t) => (
            <Paper key={t.templateId} sx={{ p: 3, borderRadius: 3, backgroundColor: "grey.50" }}>
              <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                <Box>
                  <Typography variant="h6" fontWeight={600}>{t.title}</Typography>
                  <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>{t.message}</Typography>
                  <Divider sx={{ my: 1 }} />
                  <Typography variant="body2">
                    <strong>Placeholders:</strong>{" "}
                    {Object.keys(t.placeholders).length > 0
                      ? Object.entries(t.placeholders)
                          .map(([key, value]) => `${key}${value ? `: ${value}` : ""}`)
                          .join(", ")
                      : "None"}
                  </Typography>
                </Box>

                <Box sx={{ display: "flex", gap: 1 }}>
                  <IconButton color="primary" onClick={() => setEditing(t)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton color="error" onClick={() => handleDelete(t.templateId)}>
                    <DeleteIcon />
                  </IconButton>
                </Box>
              </Box>

              {/* Edit Form */}
              {editing?.templateId === t.templateId && (
                <Paper sx={{ p: 3, mt: 2, borderRadius: 3, backgroundColor: "background.paper" }}>
                  <TemplateForm
                    initial={t}
                    onTemplateCreated={(payload: any) => handleUpdate(t.templateId, payload)}
                    onCancel={() => setEditing(null)}
                  />
                </Paper>
              )}
            </Paper>
          ))}
        </Box>
      </Container>
    </Layout>
  );
}

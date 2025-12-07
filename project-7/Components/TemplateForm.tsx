"use client";

import { useEffect, useState, FormEvent } from "react";

export type TemplatePayload = {
  title: string;
  message: string;
  placeholders: Record<string, string>;
};

type TemplateFormProps = {
  initial?: TemplatePayload | null;
  onTemplateCreated?: (payload: TemplatePayload) => Promise<void> | void;
  onCancel?: () => void;
};

export default function TemplateForm({
  initial,
  onTemplateCreated,
  onCancel,
}: TemplateFormProps) {
  const [title, setTitle] = useState(initial?.title ?? "");
  const [message, setMessage] = useState(initial?.message ?? "");
  const [placeholders, setPlaceholders] = useState<Record<string, string>>(
    initial?.placeholders ?? {}
  );

  // Extract placeholder names from message: {{UserName}}, {{ EventName }} etc.
  const extractPlaceholderNames = (text: string): string[] => {
    const matches = text.match(/{{\s*[\w]+\s*}}/g) || [];
    const names = matches.map((m) => m.replace(/[{}]/g, "").trim());
    return Array.from(new Set(names));
  };

  // Keep placeholders in sync with the message, but preserve existing values
  useEffect(() => {
    const names = extractPlaceholderNames(message);

    setPlaceholders((prev) => {
      const updated: Record<string, string> = {};
      names.forEach((name) => {
        updated[name] = prev[name] ?? "";
      });
      return updated;
    });
  }, [message]);

  const handlePlaceholderChange = (key: string, value: string) => {
    setPlaceholders((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const payload: TemplatePayload = { title, message, placeholders };

    try {
      if (onTemplateCreated) {
        await onTemplateCreated(payload);
      }

      if (!initial) {
        setTitle("");
        setMessage("");
        setPlaceholders({});
      }
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="p-5 border rounded-lg shadow bg-white"
    >
      <h2 className="text-lg font-semibold mb-4">
        {initial ? "Edit Template" : "Create New Template"}
      </h2>

      {/* Title */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Title</label>
        <input
          type="text"
          className="w-full border p-2 rounded"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
        />
      </div>

      {/* Message */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Message</label>
        <textarea
          className="w-full border p-2 rounded h-28"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          required
        />
      </div>

      {/* Placeholders (editable) */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Placeholders</label>
        {Object.keys(placeholders).length > 0 ? (
          <div className="flex flex-col gap-2">
            {Object.keys(placeholders).map((key) => (
              <div key={key} className="flex items-center gap-2">
                <span className="px-2 py-1 bg-gray-200 rounded text-sm w-32">
                  {key}
                </span>
                <input
                  type="text"
                  className="border p-1 rounded flex-1"
                  value={placeholders[key]}
                  onChange={(e) =>
                    handlePlaceholderChange(key, e.target.value)
                  }
                  placeholder={`Value for ${key}`}
                />
              </div>
            ))}
          </div>
        ) : (
          <span>None</span>
        )}
      </div>

      {/* Buttons */}
      <div className="flex gap-2">
        <button
          type="submit"
          className="px-4 py-2 bg-green-600 text-white rounded"
        >
          Save
        </button>
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            className="px-4 py-2 bg-gray-200 rounded"
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  );
}

export function formatLabel(str) {
  if (!str) return "";
  const replaced = str.replace(/_/g, " ");
  return replaced.charAt(0).toUpperCase() + replaced.slice(1);
}

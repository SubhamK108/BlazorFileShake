/** @type {import('tailwindcss').Config} */

module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: {
    extend: {
      fontFamily: {
        mono: ["Fira Code"]
      }
    }
  },
  plugins: []
};

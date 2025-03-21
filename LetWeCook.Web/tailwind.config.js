/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Views/**/*.cshtml",
        "./Areas/**/Views/**/*.cshtml",
        "./wwwroot/js/**/*.js"
    ],
    theme: {
        extend: {
            colors: {
                cream: {
                    50: '#FDFBF6',
                    100: '#F9F5EC',
                    200: '#F2EBD8',
                    300: '#EBE1C4',
                },
                teal: {
                    100: '#CCFBF1',
                    200: '#99F6E4',
                    300: '#5EEAD4',
                    500: '#14B8A6',
                    600: '#0D9488',
                    700: '#0F766E',
                    800: '#115E59',
                    900: '#134E4A',
                },
            },
        },
    },
    plugins: [],
};

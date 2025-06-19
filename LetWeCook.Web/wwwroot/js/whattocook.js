import { GoogleGenerativeAI } from "https://esm.sh/@google/generative-ai";

// Initialize the Google Generative AI client with the API key from .env
const genAI = new GoogleGenerativeAI("AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94");

const INGREDIENT_POOL = [
    "Hot sauce",
    "Mung beans",
    "Apples",
    "Hemp seeds",
    "Blueberries",
    "Cabbage (green, red, Napa)",
    "Marjoram",
    "Barbecue sauce",
    "Goose eggs",
    "Mushrooms (button, portobello, shiitake, cremini)",
    "Figs",
    "Hazelnuts",
    "Rosemary",
    "Shrimp",
    "Oranges",
    "Brown sugar",
    "Chicken thighs",
    "Chicken breast",
    "Cod",
    "Ketchup",
    "Octopus",
    "Molasses",
    "Almonds",
    "Nutmeg",
    "Black beans",
    "Black pepper",
    "White rice",
    "Camembert",
    "Quail",
    "Pheasant",
    "Chia seeds",
    "Blackberries",
    "Cinnamon",
    "Coffee",
    "Quail eggs",
    "Tea (black, green, herbal)",
    "Mayonnaise",
    "Radishes",
    "Tomatoes",
    "Papaya",
    "Cocoa powder",
    "Greek yogurt",
    "Natto",
    "Tahini",
    "Edamame",
    "Worcestershire sauce",
    "Tarragon",
    "Tuna",
    "Bread flour",
    "Cheddar cheese",
    "Lamb",
    "Fava beans",
    "Catfish",
    "Chickpeas",
    "Pecans",
    "Oat milk",
    "Bananas",
    "Chicken eggs",
    "Barley",
    "Kiwi",
    "Pesto",
    "Rabbit",
    "Allspice",
    "Mozzarella cheese",
    "Parsley",
    "Whole milk",
    "Farro",
    "Soy sauce",
    "Blue cheese",
    "Bay leaves",
    "Sparkling water",
    "Snapper",
    "Mackerel",
    "Walnuts",
    "Scallions (green onions)",
    "Hemp milk",
    "Scallops",
    "Flaxseed milk",
    "Vanilla extract",
    "Sunflower seeds",
    "Wild rice",
    "Raspberries",
    "Oysters",
    "Skim milk",
    "Yeast",
    "Coriander",
    "Cayenne pepper",
    "Orange juice",
    "Pumpkin seeds",
    "Plums",
    "Onions",
    "Oats",
    "Almond milk",
    "Cherries",
    "Sour cream",
    "Arugula",
    "Cashew milk",
    "Vegetable oil",
    "Whole wheat flour",
    "Parmesan cheese",
    "Cardamom",
    "Baking soda",
    "Paprika",
    "Water",
    "Kidney beans",
    "Apple juice",
    "Turkey breast",
    "Coconut milk",
    "Celery",
    "Sauerkraut",
    "Potatoes",
    "Baking powder",
    "Garlic",
    "Star anise",
    "Pea protein milk",
    "White pepper",
    "Trout",
    "Ginger",
    "Bone broth",
    "Canola oil",
    "Strawberries",
    "Avocado",
    "Cantaloupe",
    "Sweet potatoes",
    "Honey",
    "Clams",
    "Pinto beans",
    "Dates",
    "Rye",
    "Pistachios",
    "Anchovies",
    "Kimchi",
    "Teriyaki sauce",
    "Turkey thighs",
    "Grapeseed oil",
    "Stevia",
    "Oregano",
    "Mussels",
    "Duck",
    "Watermelon",
    "Mustard seeds",
    "Prawns",
    "Cottage cheese",
    "Basmati rice",
    "Shallots",
    "Cashews",
    "Lobster",
    "Vegan cheese",
    "Mustard",
    "Tilapia",
    "Swiss cheese",
    "Feta cheese",
    "Chives",
    "Chili powder",
    "Sports drinks",
    "Provolone",
    "Split peas",
    "Hummus",
    "Peas",
    "Powdered sugar",
    "Macadamia milk",
    "Kombucha",
    "Pomegranate",
    "Milk",
    "Whole chicken",
    "Cornish hen",
    "Macadamia nuts",
    "Pineapple",
    "Leeks",
    "Cream cheese",
    "Chicken drumsticks",
    "Peanut oil",
    "Beets",
    "Salsa",
    "Brie",
    "Condensed milk",
    "Agave syrup",
    "Crab",
    "Lemons",
    "Sardines",
    "Pork",
    "Green beans",
    "Sesame oil",
    "Squid (Calamari)",
    "Zucchini",
    "Broccoli",
    "Nectarines",
    "Haddock",
    "Tempeh",
    "Vegetable broth",
    "Flaxseeds",
    "Evaporated milk",
    "Heavy cream",
    "Eggplant",
    "Veal",
    "Mint",
    "Mango",
    "Goat",
    "Lima beans",
    "Halibut",
    "Half and half",
    "Semolina",
    "Grits",
    "Turmeric",
    "Asparagus",
    "Butter",
    "Gouda cheese",
    "Mascarpone",
    "Coconut",
    "Limes",
    "Sesame seeds",
    "Artichokes",
    "Couscous",
    "Lemonade",
    "Cloves",
    "Coconut water",
    "White sugar",
    "Grapes",
    "Bison",
    "Chicken broth",
    "Carrots",
    "Gelatin",
    "Cucumber",
    "Basil",
    "Dill",
    "Brussels sprouts",
    "Olive oil",
    "Avocado oil",
    "Salmon",
    "Fish stock",
    "Venison",
    "Collard greens",
    "Honeydew melon",
    "Salt",
    "Jasmine rice",
    "Cornstarch",
    "Brazil nuts",
    "Guacamole",
    "Ghee",
    "Ricotta cheese",
    "Beef",
    "Lentils (red, green, black)",
    "Rice milk",
    "Beef broth",
    "Chicken wings",
    "Flaxseed oil",
    "Yogurt",
    "Sage",
    "Whole turkey",
    "Sumac",
    "Chocolate chips",
    "Miso",
    "Kale",
    "Quinoa",
    "Millet",
    "Duck eggs",
    "Bulgur",
    "Monk fruit sweetener",
    "Thyme",
    "Cilantro",
    "Lettuce (romaine, iceberg, butterhead)",
    "All-purpose flour",
    "Fennel seeds",
    "Spinach",
    "Almond extract",
    "Sunflower oil",
    "Cumin",
    "Maple syrup",
    "Buttermilk",
    "Poppy seeds",
    "Bell peppers (red, green, yellow, orange)",
    "Coconut yogurt",
    "Peaches",
    "Cauliflower",
    "Buckwheat",
    "Soy milk",
    "Whipping cream",
    "Cornmeal",
    "White beans",
    "Brown rice",
    "Saffron",
    "Coconut oil"
];

function buildPrompt(ingredientPool) {
    return `
You are a cooking assistant. Your task is to analyze a set of images and detect ingredients from them.

Only identify ingredients that are in this approved list:

${ingredientPool.join(", ")}

Respond ONLY with a JSON array of detected ingredient names from the approved list. No explanation, no extra formatting.

Example response:
["Tomatoes", "Cilantro", "Garlic"]

Now, based on the given images, what ingredients are shown?
`;
}

export async function analyzeImages(files) {
    const model = await genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

    const prompt = buildPrompt(INGREDIENT_POOL);

    // Convert images to base64 and wrap in Part for Gemini Vision
    const imageParts = await Promise.all(
        files.map(async (file) => ({
            inlineData: {
                mimeType: file.type,
                data: await fileToBase64(file),
            },
        }))
    );

    const result = await model.generateContent({
        contents: [
            {
                role: "user",
                parts: [
                    { text: prompt },
                    ...imageParts
                ]
            }
        ]
    });

    const response = await result.response.text();

    try {
        // Try to parse just the JSON array from the response
        const jsonStart = response.indexOf("[");
        const jsonEnd = response.lastIndexOf("]");
        const jsonString = response.slice(jsonStart, jsonEnd + 1);
        return JSON.parse(jsonString);
    } catch (err) {
        console.error("⚠️ Failed to parse Gemini response:", response);
        return [];
    }
}

function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const base64 = reader.result.split(',')[1];
            resolve(base64);
        };
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
}


$(document).ready(function () {
    let imageFiles = [];

    $.ajax({
        url: '/api/ingredients/summary',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            // Map the response to extract only ingredient names
            const INGREDIENT_POOL = data.map(ingredient => ingredient.name);

            // Log to verify the result
            console.log('Updated INGREDIENT_POOL:', INGREDIENT_POOL);
        },
        error: function (xhr) {
            console.error("Error fetching ingredient summary:", {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText
            });

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: xhr.responseJSON?.message || 'Failed to load ingredient summary. Please try again later.',
                confirmButtonColor: '#dc3545'
            });
        }

    });

    $('#add-image').on('click', function () {
        $('#image-input').click();
    });

    $('#image-input').on('change', function (e) {
        const files = Array.from(e.target.files);
        files.forEach(file => {
            const reader = new FileReader();
            reader.onload = function (event) {
                const imgUrl = event.target.result;

                const index = imageFiles.length;
                imageFiles.push(file);

                const imgHtml = `
          <div class="relative w-32 h-32 rounded-xl overflow-hidden border-2 border-orange-300 bg-white shadow" data-index="${index}">
            <img src="${imgUrl}" alt="preview" class="object-cover w-full h-full" />
            <button class="remove-image absolute top-1 right-1 bg-red-600 text-white rounded-full w-6 h-6 text-xs flex items-center justify-center hover:bg-red-700">
              ×
            </button>
          </div>
        `;

                $('#image-preview-container').prepend(imgHtml);
            };
            reader.readAsDataURL(file);
        });

        // Clear input to allow re-upload of same file later
        $(this).val('');
    });

    // Remove image
    $('#image-preview-container').on('click', '.remove-image', function () {
        const parent = $(this).closest('[data-index]');
        const index = parseInt(parent.data('index'));

        imageFiles[index] = null; // Mark as removed
        parent.remove();
    });

    // Get all active image files (skip removed)
    function getUploadedImages() {
        return imageFiles.filter(f => f !== null);
    }

    $('#check-images-btn').on('click', async function () {
        $('#spinner-overlay').removeClass('hidden'); // Show spinner

        try {
            const images = getUploadedImages(); // returns File[]
            detectedIngredients = await analyzeImages(images); // e.g., ['onion', 'carrot']
            console.log('Detected ingredients:', detectedIngredients);
            const relatedBasic = getTopRelatedRecipes(detectedIngredients, ingredientIndex, recipes, 5);
            const fullRecipes = await fetchRecipeDetailsForMatches(relatedBasic);

            renderFloatingRecipeCards(fullRecipes, detectedIngredients);
            // scroll to bottom of the page with some animation time
            $('html, body').animate({
                scrollTop: $(document).height()
            }, 1000);

        } catch (error) {
            console.error('❌ Error during image analysis or recipe rendering:', error);
        } finally {
            $('#spinner-overlay').addClass('hidden');
        }
    });

    loadIngredientData();
    fetchIngredientOverviews();


});

let ingredientIndex = {};
let recipes = [];
let detectedIngredients = [];
let ingredientOverviews = [];

loadIngredientData();
fetchIngredientOverviews();

async function loadIngredientData() {
    try {
        const [indexRes, recipesRes] = await Promise.all([
            fetch('/data/ingredient-index.json'),
            fetch('/data/recipes.json')
        ]);

        if (!indexRes.ok || !recipesRes.ok) {
            throw new Error('Failed to load one or both data files.');
        }

        ingredientIndex = await indexRes.json();
        recipes = await recipesRes.json();
    } catch (error) {
        console.error("❌ Error loading ingredient/recipe data:", error);
    }
}

function fetchIngredientOverviews() {
    $.ajax({
        url: '/api/ingredients/summary',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            ingredientOverviews = data;
        },
        error: function (xhr) {
            console.error("Error fetching ingredient summary:", {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText
            });

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: xhr.responseJSON?.message || 'Failed to load ingredient summary. Please try again later.',
                confirmButtonColor: '#dc3545'
            });
        }

    });
}

function getTopRelatedRecipes(detectedIngredients, ingredientIndex, recipes, maxMissing = 2) {
    const matchedRecipeMap = new Map();
    const detectedLower = detectedIngredients.map(i => i.toLowerCase());

    for (const name of detectedLower) {
        const recipeIds = ingredientIndex[name];
        if (!recipeIds) continue;

        for (const id of recipeIds) {
            if (!matchedRecipeMap.has(id)) matchedRecipeMap.set(id, new Set());
            matchedRecipeMap.get(id).add(name);
        }
    }

    const results = [];

    for (const recipe of recipes) {
        const matchedSet = matchedRecipeMap.get(recipe.id);
        const matchedCount = matchedSet ? matchedSet.size : 0;
        const totalRequired = recipe.ingredients.length;
        const missingCount = totalRequired - matchedCount;

        if (matchedCount > 0 && missingCount <= maxMissing) {
            results.push({
                id: recipe.id,
                matchedCount,
                missingCount,
                totalRequired,
                matchedIngredientNames: Array.from(matchedSet)
            });
        }
    }

    results.sort((a, b) => {
        if (a.missingCount !== b.missingCount) return a.missingCount - b.missingCount;
        return b.matchedCount - a.matchedCount;
    });

    return results;
}

async function fetchRecipeDetailsForMatches(matchedBasicRecipes) {
    const details = await Promise.all(
        matchedBasicRecipes.map(async base => {
            try {
                const res = await fetch(`/api/recipe-details/${base.id}`);
                if (!res.ok) throw new Error(`Failed to fetch details for recipe ${base.id}`);
                const full = await res.json();
                return { ...full, matchedIngredientNames: base.matchedIngredientNames };
            } catch (e) {
                console.error("❌ Error fetching recipe details:", e);
                return null;
            }
        })
    );

    return details.filter(Boolean); // Remove failed ones
}

function renderFloatingRecipeCards(recipes, detectedIngredients) {
    const $container = $("#recipe-results").empty();
    const containerWidth = $container.width();
    const containerHeight = $container.height();

    const detectedLower = detectedIngredients.map(name => name.toLowerCase());

    if (!recipes.length) {
        $container.html(`
            <div class="w-full h-full flex flex-col items-center justify-center text-center text-orange-700 gap-4">
                <div class="text-6xl">😔</div>
                <p class="text-xl font-semibold">No recipes matched your detected ingredients.</p>
                <p class="text-sm text-gray-500 max-w-md">Try uploading more clear or relevant food photos, or consider using different ingredients.</p>
            </div>
        `);
        return;
    }

    const cards = recipes.map(recipe => {
        const $card = $(`
            <div class="absolute bg-white p-4 rounded-xl shadow cursor-pointer select-none transition-all duration-200 border border-transparent hover:border-emerald-500 hover:shadow-lg" style="width: 320px; z-index: 10;"></div>
        `);

        $card.on('click', () => {
            window.location.href = `/Cooking/Recipe/Details/${recipe.id}`; // Adjust route if needed
        });

        $card.on('mouseenter', function () {
            $(this).css('z-index', 1000); // bring to front
        });
        $card.on('mouseleave', function () {
            $(this).css('z-index', 10); // reset to normal
        });

        $container.append($card);

        const matched = [];
        const missing = [];

        recipe.ingredients.forEach(ing => {
            const ingName = ing.ingredientName.toLowerCase().trim();
            if (detectedLower.includes(ingName)) matched.push(ing);
            else missing.push(ing);
        });

        $card.html(`
            <div class="flex flex-col gap-2">
                <div class="flex gap-2">
                    <img src="${recipe.coverImage}" alt="${recipe.name}" class="w-24 h-24 object-cover rounded" />
                    <div class="flex flex-col justify-between flex-1">
                        <h3 class="font-semibold text-base">${recipe.name}</h3>
                        <p class="text-sm text-gray-600 line-clamp-2">${recipe.description}</p>
                        <p class="text-sm text-yellow-600 font-medium">★ ${recipe.averageRating.toFixed(1)} (${recipe.totalRatings} ratings)</p>
                    </div>
                </div>

                <div class="grid grid-cols-2 gap-2 text-sm">
                    <div>
                        <p class="font-semibold text-green-700 mb-1">✓ You Have</p>
                        ${matched.length ? matched.map(ing => `
                            <div class="flex items-center gap-1 mb-1">
                                <img src="${ing.coverImageUrl}" class="w-4 h-4 rounded" />
                                <span>${ing.quantity} ${ing.unit} ${ing.ingredientName}</span>
                            </div>`).join('') : '<p class="text-gray-500">None</p>'}
                    </div>
                    <div>
                        <p class="font-semibold text-red-700 mb-1">✗ Missing</p>
                        ${missing.length ? missing.map(ing => `
                            <div class="flex items-center gap-1 mb-1">
                                <img src="${ing.coverImageUrl}" class="w-4 h-4 rounded" />
                                <span>${ing.quantity} ${ing.unit} ${ing.ingredientName}</span>
                            </div>`).join('') : '<p class="text-green-600">All available</p>'}
                    </div>
                </div>

                <div class="flex flex-wrap justify-between text-xs text-gray-600 border-t pt-2 mt-2">
                    <span><strong>Prep:</strong> ${recipe.prepareTime}m</span>
                    <span><strong>Cook:</strong> ${recipe.cookTime}m</span>
                    <span><strong>Total:</strong> ${recipe.totalTime}m</span>
                    <span><strong>Serves:</strong> ${recipe.servings}</span>
                    <span><strong>Level:</strong> ${recipe.difficulty}</span>
                </div>

                <div class="flex items-center gap-2 mt-2 border-t pt-2">
                    <img src="${recipe.authorProfile.profilePicUrl}" class="w-6 h-6 rounded-full" />
                    <span class="text-xs text-gray-500">${recipe.authorProfile.name}</span>
                </div>
            </div>
        `);

        const width = $card.outerWidth();
        const height = $card.outerHeight();

        return {
            $el: $card,
            x: Math.random() * (containerWidth - width),
            y: Math.random() * (containerHeight - height),
            dx: (Math.random() - 0.5) * 1.5,
            dy: (Math.random() - 0.5) * 1.5,
            width,
            height
        };
    });

    function animate() {
        for (const card of cards) {
            card.dx += (Math.random() - 0.5) * 0.05;
            card.dy += (Math.random() - 0.5) * 0.05;

            card.dx = Math.max(Math.min(card.dx, 2), -2);
            card.dy = Math.max(Math.min(card.dy, 2), -2);

            card.x += card.dx;
            card.y += card.dy;

            if (card.x < 0 || card.x > containerWidth - card.width) card.dx *= -1;
            if (card.y < 0 || card.y > containerHeight - card.height) card.dy *= -1;

            card.$el.css({ left: `${card.x}px`, top: `${card.y}px` });
        }

        requestAnimationFrame(animate);
    }

    animate();
}

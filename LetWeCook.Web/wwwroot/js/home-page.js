

$(document).ready(function () {
    const $el = $('#fire-border');
    const shadows = [
        "0 0 8px 3px #ff4500, 0 0 15px 6px #ff8c00, 0 0 25px 10px #b22222, inset 0 0 40px 15px rgba(255,140,0,0.9)",
        "0 0 10px 4px #ff5a00, 0 0 18px 8px #ff9a1a, 0 0 28px 12px #d43b1a, inset 0 0 50px 20px rgba(255,165,0,1)",
        "0 0 12px 5px #ff6a00, 0 0 20px 9px #ffb330, 0 0 32px 15px #d4532a, inset 0 0 55px 25px rgba(255,180,20,1)"
    ];
    let idx = 0;

    setInterval(() => {
        $el.css('box-shadow', shadows[idx]);
        idx = (idx + 1) % shadows.length;
    }, 800);
});
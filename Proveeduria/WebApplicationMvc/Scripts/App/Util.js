function run_waitMe(el, effect, texto) {
    fontSize = '';
    maxSize = '';
    textPos = 'vertical';
    el.waitMe({
        effect: effect,
        text: texto,
        bg: 'rgba(255,255,255,0.7)',
        color: '#000',
        maxSize: 30,
        source: 'img.svg',
        textPos: textPos,
        fontSize: fontSize,
        onClose: function () { }
    });
}
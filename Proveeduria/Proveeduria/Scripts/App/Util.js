function run_waitMe(el, texto) {
    fontSize = '';
    maxSize = '';
    textPos = 'vertical';
    el.waitMe({
        effect: 'roundBounce',
        text: texto,
        bg: 'rgba(255,255,255,0.7)',
        color: '#000',
        maxSize: 30,
        //source: 'img.svg',
        textPos: textPos,
        fontSize: fontSize,
        onClose: function () { }
    });
}

function serializaForma(pforma) {
    var pforma = pforma.serializeArray();
    var retorno = {};
    for (var a = 0; a < pforma.length; a++) retorno[pforma[a].name] = pforma[a].value;
    return retorno;
}


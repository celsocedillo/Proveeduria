const EnumTipoMovimiento = {
    REQUISICION_BODEGA: 2,
    INGRESO_DE_ORDEN_DE_COMPRA: 4,
    AJUSTE_DE_BODEGA_POR_EGRESO: 11,
    AJUSTE_DE_BODEGA_POR_INGRESO: 10
}

const EnumEstadoRegistro = {
    Activo: 'A',
    Inactivo: 'I'
}

function getMapValue(obj, key) {
    if (obj.hasOwnProperty(key))
        return obj[key];
    //throw new Error("Invalid map key.");
}
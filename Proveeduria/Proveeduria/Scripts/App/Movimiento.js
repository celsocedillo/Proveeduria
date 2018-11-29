var tabItems;
var tipoMovimientoSeleccionado;
var ldata;
var loc;
var movimiento;
var movimientoDetalle = [];

function agregarItem(item) {
    if (item.ID_ITEM !== undefined) {
        producto = item.DESCRIPCION;
        var desabilitado="";

        if ($("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.INGRESO_DE_ORDEN_DE_COMPRA) {
            desabilitado = "disabled";
        }
        var cantidad = `<div class="col-md-1">
                            <input id=txtcant${item.ID_ITEM} name="detalle-cantidad" value=${item.CANTIDAD_MOVIMIENTO} style="width: 75px;" class="form-control text-right txtcant no-spin" min=0 step="any" required ${desabilitado}>
                        </div>`;
        var row = []
        row.push({
            "CODIGO": item.CODIGO,
            "DESCRIPCION": item.ITEM,
            "CANTIDAD_SOLICITADA": item.CANTIDAD_SOLICITADA,
            "CANTIDAD_MOVIMIENTO": cantidad,
            "STOCK_ACTUAL": item.STOCK_ACTUAL,
            "ID_DETALLE": 0,
            "UNIDAD" : null,
            "ID_ITEM": item.ID_ITEM,
            "COSTO_MOVIMIENTO": item.COSTO_MOVIMIENTO
        });
        //var index = tabItems.row.add(row).draw(false).index();
        tabItems.rows.add(row).draw()
        //tabItems.columns.adjust().draw();
        $("#txtcant" + item.ID_ITEM).on('keypress', function (e) {
            if (e.which === 13) {
                var cantidad
                if ($(this).val() === "") {
                    cantidad = 0;
                } else {
                    cantidad = $(this).val();
                }
                if (cantidad > 0) {
                    $('#sltItem').val('').trigger('change');
                    $("#sltItem").select2('open');
                }
            }
        })

        $("#txtcant" + item.ID_ITEM).on('change', function (e) {
            if ($(this).val() > item.STOCK_ACTUAL && ($("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.AJUSTE_DE_BODEGA_POR_EGRESO || $("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.REQUISICION_BODEGA)) {
                swal({
                    type: 'ERROR',
                    type: 'error',
                    text: 'El item no tiene stock suficiente',
                    confirmButtonColor: '#00BCD4'
                });
                $(this).val(0);
                $(this).focus();
            }
        });

        $("#txtcant" + item.ID_ITEM).focus();
    }
    else {
        swal({
            type: 'ERROR',
            type: 'error',
            text: 'Por favor seleccione un producto',
            confirmButtonColor: '#00BCD4'
        });
    }
}

function BuscarSolicitudAutorizada() {
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/GetListaSolicitudAutorizada",
        beforeSend: function () {
            run_waitMe($(".wrapper"), '', 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                ldata = data.data;
                tabDocumentoRelacionado.clear();
                tabDocumentoRelacionado.rows.add(data.data);
                tabDocumentoRelacionado.draw();
                $("#titDocumentoRelacionado").text("Solicitudes de requisición de bodega");
                $("#dlgDocumentoRelacionado").modal('toggle');
            }
            $(".wrapper").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".wrapper").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

function BuscarOrdenCompra() {
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/GetListaOrdenCompra",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'win8', 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                loc = data.data;
                tabOrdenCompra.clear();
                tabOrdenCompra.rows.add(data.data);
                tabOrdenCompra.draw();
                $("#dlgOrdenCompra").modal('toggle');
            }
            $(".box-body").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

function DetalleSolicitud(pid_movimiento_relacionado) {
    var parametros = JSON.stringify(
        {
            pid_movimiento: pid_movimiento_relacionado
        });
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/GetDetalleSolicitudAutorizada",
        beforeSend: function () {
            run_waitMe($(".wrapper"), 'win8', 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                tabItems.clear();
                ldata = data.data;
                for (i in ldata) {
                    var item = {
                        ID_ITEM: ldata[i].ID_ITEM,
                        CODIGO: ldata[i].CODIGO,
                        ITEM: ldata[i].ITEM,
                        UNIDAD: null,
                        CANTIDAD_SOLICITADA: ldata[i].CANTIDAD,
                        CANTIDAD_MOVIMIENTO: ldata[i].CANTIDAD,
                        ID_DETALLE: 0,
                        STOCK_ACTUAL: ldata[i].STOCK_ACTUAL,
                        COSTO_MOVIMIENTO: 0
                    }
                    agregarItem(item);
                }
                column = tabItems.column(5);
                column.visible(!column.visible());
            }
            $(".wrapper").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".wrapper").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

function DetalleOrdenCompra(panio, pnumero_orden) {
    var parametros = JSON.stringify(
        {
            panio: panio,
            pnumero_orden: pnumero_orden
        });
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/GetDetalleOrdenCompra",
        beforeSend: function () {
            run_waitMe($(".wrapper"), 'win8', 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                tabItems.clear();
                ldata = data.data;
                for (i in ldata) {

                    var item = {
                        ID_ITEM: ldata[i].ID_ITEM,
                        CODIGO: ldata[i].CODIGO,
                        ITEM: ldata[i].ITEM,
                        UNIDAD: null,
                        CANTIDAD_SOLICITADA: ldata[i].CANTIDAD,
                        CANTIDAD_MOVIMIENTO: ldata[i].CANTIDAD,
                        ID_DETALLE: 0,
                        STOCK_ACTUAL: ldata[i].STOCK_ACTUAL,
                        COSTO_MOVIMIENTO: ldata[i].VALOR_UNITARIO
                    }
                    agregarItem(item);
                }
                column = tabItems.column(5);
                column.visible(!column.visible());
            }
            $(".wrapper").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".wrapper").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

function ValidaPantalla() {
    $('#frmMovimiento').parsley().validate();
    if ($('#frmMovimiento').parsley().isValid()) {
        var validarDetalle = true;
        var itemsconcero = false;
        var stockinsuficiente = false;
        movimientoDetalle = [];
        //Se validan los items y se los agrega en un arreglo
        if (tabItems.rows().count() > 0) {
            $("#tabItems tbody tr").each(function () {
                var fila = $(this).closest('tr');
                linea_data = tabItems.row(tabItems.row(fila).index());
                if ($('input[name="detalle-cantidad"]', fila).val() > 0) {
                    if (($('input[name="detalle-cantidad"]', fila).val() > linea_data.data()["STOCK_ACTUAL"])
                        && ($("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.REQUISICION_BODEGA || $("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.AJUSTE_DE_BODEGA_POR_EGRESO)
                       )
                    {
                        $('input[name="detalle-cantidad"]', fila).addClass("parsley-error");
                        stockinsuficiente = true;
                    }
                    //Si la cantidad que se pide es mayor a cero entonces se lo agrega en el arreglo
                    movimientoDetalle.push({
                        "ID_DETALLE": 0,
                        "ID_ITEM": linea_data.data()['ID_ITEM'],
                        "CANTIDAD_MOVIMIENTO": $('input[name="detalle-cantidad"]', fila).val(),
                        "COSTO_MOVIMIENTO": linea_data.data()['COSTO_MOVIMIENTO'],
                        "ESTADO": "A"
                    });
                } else {
                    //Si la cantidad es 0 entonces se deja una alerta 
                    itemsconcero = true;
                    if ($("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.REQUISICION_BODEGA) {
                        movimientoDetalle.push({
                            "ID_DETALLE": 0,
                            "ID_ITEM": linea_data.data()['ID_ITEM'],
                            "CANTIDAD_MOVIMIENTO": $('input[name="detalle-cantidad"]', fila).val(),
                            "COSTO_MOVIMIENTO": linea_data.data()['COSTO_MOVIMIENTO'],
                            "ESTADO": "A"
                        });
                    } else {
                        validarDetalle = false;
                    }
                }
            });


            
            if (stockinsuficiente == true) {
                return 3;
            } else if (itemsconcero == true && $("#ID_TIPO_MOVIMIENTO").val() == 2) {
                //Si es una orden de requisicion se permite despachar cantidades con 0
                return 1;
                //Si es otro tipo de movimiento no se permite cantidades con 0
            } else if (itemsconcero == true) {
                return 2;
            }  
            return 0;
        }
    }
}

function Grabar() {
    movimiento = {
                    "ID_MOVIMIENTO": 0,
                    "ID_TIPO_MOVIMIENTO": $("#ID_TIPO_MOVIMIENTO").val(),
                    "OBSERVACION": $("#OBSERVACION").val(),
                    "ID_MOVIMIENTO_RELACION": $("#ID_MOVIMIENTO_RELACION").val(),
                    "ANIO_DOCUMENTO_REFERENCIA": $("#ANIO_DOCUMENTO_REFERENCIA").val(),
                    "NUMERO_DOCUMENTO_REFERENCIA": $("#NUMERO_DOCUMENTO_REFERENCIA").val(),
                    "EPRTA_MOVIMIENTO_DETALLE": movimientoDetalle
                }
    var parametros = JSON.stringify({
        pmovimiento: movimiento
    });

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/Grabar",
        beforeSend: function () {
            run_waitMe($(".box-primary"), 'Grabando...');
        },
        success: function (data) {
            if (data.resultado === "success") {
                movimientoId = data.data;
                Imprimir();
                $("#btnGrabar").prop("disabled", true);
            }
            else {
                swal({
                    type: data.resultado,
                    text: 'Error al grabar los datos. ' + data.mensaje,
                    confirmButtonColor: '#00BCD4'
                });
            }
            $(".box-primary").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-primary").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

function Imprimir() {
    if ($("#ID_TIPO_MOVIMIENTO").val() == 2) {
        $("#modImpRequisicion").modal("show");
        var id = $(this).data("id");
        $('#tagRequisicion').attr("src", "/Solicitud/ViewPDF/" + idRequisicion);
    }
    $("#modal-dialog-print-pdf").modal("show");
    $('#TagEmbed').attr("src", "/Movimiento/ViewPDF/" + movimientoId);

}

var Movimiento = function () {
    return {
        init: function () {

            tabDocumentoRelacionado= $('#tabDocumentoRelacionado').DataTable({
                "paging": false,
                "searching": true,
                "info": false,
                "autoWidth" : false,
                //"scrollY": "200px",
                "select": { style: 'single' },
                "data": ldata,
                "columnDefs": [
                    { "targets": 0, "visible": false},
                    { "targets": 1, "width": 30 },
                    { "targets": 2, "width": 50 },
                    { "targets": 3, "width": 250 },
                    { "targets": 4, "width": 250 },
                    { "targets": 5, "width": 50 },
                    { "targets": 6, "visible": false}
                ],
                "columns": [
                    { "data": 'ID_MOVIMIENTO' },
                    { "data": 'ANIO' },
                    { "data": 'NUMERO_MOVIMIENTO' },
                    { "data": 'DEPARTAMENTO' },
                    { "data": 'EMPLEADO' },
                    { "data": 'FECHA' },
                    { "data": 'USUARIO_SOLICITA' }
                ]
            });

            tabOrdenCompra = $('#tabOrdenCompra').DataTable({
                "paging": false,
                "searching": true,
                "info": false,
                "autoWidth": false,
                "select": { style: 'single' },
                "data": loc,
                "columnDefs": [
                    { "targets": 0, "width": 30 },
                    { "targets": 1, "width": 30 },
                    { "targets": 2, "width": 350 },
                    { "targets": 3, "width": 50 },
                    { "targets": 4, "width": 50 }
                ],
                "columns": [
                    { "data": 'ANIO' },
                    { "data": 'NUMERO_MOVIMIENTO' },
                    { "data": 'PROVEEDOR' },
                    { "data": 'FECHA' },
                    { "data": 'FACTURA' }
                ]
            });
            
            tabItems= $('#tabItems').DataTable({
                "autoWidth": false,
                "paging": false,
                "columnDefs":
                    [
                        { "targets": [6], "checkboxes": { 'selectRow': false } },
                        { "targets": [7], "visible": false },
                        { "targets": [8], "visible": false }
                    ],
                "columns": [
                    { "data": 'CODIGO' },
                    { "data": 'DESCRIPCION' },
                    { "data": 'CANTIDAD_SOLICITADA' },
                    { "data": 'CANTIDAD_MOVIMIENTO' },
                    { "data": 'STOCK_ACTUAL' },
                    { "data": 'UNIDAD' },
                    { "data": 'ID_DETALLE' },
                    { "data": 'ID_ITEM' },
                    { "data": 'COSTO_MOVIMIENTO' }
                ]
            });

            column = tabItems.column(2);

            if ($("#ID_TIPO_MOVIMIENTO").val() == 2) {
                column.visible(true);
                $("#divRequisicion").css('display', 'block');
                $("#divOrdenCompra").css('display', 'none');

            } else if($("#ID_TIPO_MOVIMIENTO").val() == 4) {
                column.visible(false);
                $("#divRequisicion").css('display', 'none');
                $("#divOrdenCompra").css('display', 'block');

            } else            
            {
                column.visible(false);
                $("#divRequisicion").css('display', 'none');
                $("#divOrdenCompra").css('display', 'none');

            }

            $('#sltItem').select2({
                language: 'es',
                reverse: false,
                ajax: {
                    url: '/Item/SearchItemsFiltro',
                    dataType: "json",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    quietMillis: 100,
                    data: function (params) {
                        let parametros = JSON.stringify({ "filtro": params.term, "pagina": 10 });
                        return parametros;
                    },
                    processResults: function (data, params) {
                        var array = data.items;
                        return { results: array };
                    }
                },
                escapeMarkup: function (markup) { return markup; },
                templateResult: function (repo) {
                    return "(" + repo.CODIGO + ") " + repo.DESCRIPCION + " [" +repo.STOCK_ACTUAL + "]";
                },
                templateSelection: function (container) {
                    $(container.element).attr("data-codigo", container.CODIGO);
                    $(container.element).attr("data-item", container.DESCRIPCION);
                    $(container.element).attr("data-unidad", container.MEDIDA);
                    $(container.element).attr("data-idItem", container.id);
                    $(container.element).attr("data-stockactual", container.STOCK_ACTUAL);
                    return "(" + container.CODIGO + ") " + container.DESCRIPCION + " [" + container.STOCK_ACTUAL + "]";
                }
            });

            $('#sltItem').on('select2:select', function (e) {
                $("#btnAgregarItem").focus();
            });

            $("#btnCancelar").on('click', function () {
                window.location.href = '/Movimiento/ListaMovimiento';
            });

            $('#ID_TIPO_MOVIMIENTO').on('select2:select', function (e) {
                tipoMovimientoSeleccionado = e.params.data.id;

            });

            var tipoMovimientoPrevio = $("#ID_TIPO_MOVIMIENTO").val();
            $("#ID_TIPO_MOVIMIENTO").on('select2:open', function (e) {
                tipoMovimientoPrevio = $(this).val();
            });

            $("#ID_TIPO_MOVIMIENTO").on('change', function (e) {
                if (tipoMovimientoPrevio != $(this).val()) {
                    swal({
                        text: 'Si cambia el tipo de movimiento se borraran los items ingresados. ¿Desea continuar?',
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#00BCD4',
                        cancelButtonColor: '#EF5350',
                        confirmButtonText: 'Aceptar',
                        cancelButtonText: 'Cancelar'
                    }).then(function () {
                        tabItems.clear().draw();
                        $("#lblSolicitudRequisicion").text("");
                        $("#lblFechaAutorizacion").text("");
                        $("#lblUsuarioSolicita").text("");
                        $("#lblDptoSolicita").text("");
                        $("#lblOrdenCompra").text("");
                        $("#lblProveedor").text("");
                        $("#lblFactura").text("");
                        $("#lblFechaFactura").text("");
                        if (tipoMovimientoSeleccionado == EnumTipoMovimiento.REQUISICION_BODEGA) {
                            $("#divBuscaRelacion").css('display', 'block');
                            $("#divAgregar").css('display', 'block');
                            $("#btnEliminarItem").css('display', 'none');
                            $("#divRequisicion").css('display', 'block');
                            $("#divOrdenCompra").css('display', 'none');
                        } else if (tipoMovimientoSeleccionado == EnumTipoMovimiento.INGRESO_DE_ORDEN_DE_COMPRA) {
                            $("#divBuscaRelacion").css('display', 'block');
                            $("#divAgregar").css('display', 'none');
                            $("#divRequisicion").css('display', 'none');
                            $("#divOrdenCompra").css('display', 'block');
                        } else {
                            $("#divBuscaRelacion").css('display', 'none');
                            $("#divAgregar").css('display', 'block');
                            $("#divRequisicion").css('display', 'none');
                            $("#divOrdenCompra").css('display', 'none');
                            $("#btnEliminarItem").css('display', 'inline-block');
                        }

                        column = tabItems.column(2);

                        if (tipoMovimientoSeleccionado == 2) {
                            column.visible(true);
                        } else {
                            column.visible(false);
                        }
                        }, function (dismiss) {
                            if (dismiss == 'cancel') {
                                $("#ID_TIPO_MOVIMIENTO").val(tipoMovimientoPrevio).trigger('change.select2');
                            }
                    });
                }
            });

            $("#btnGrabar").on('click', function () {
                swal({
                    text: '¿Seguro desea grabar el movimiento de bodega?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#00BCD4',
                    cancelButtonColor: '#EF5350',
                    confirmButtonText: 'Aceptar',
                    cancelButtonText: 'Cancelar'
                }).then(function () {
                    var validacion = ValidaPantalla();
                    if (validacion == 0) {
                        Grabar();
                    } else if (validacion == 1) {
                        swal({
                            text: 'Existen items con cantidad 0 o menor a 0, ¿Seguro desea continuar?',
                            type: 'info',
                            showCancelButton: true,
                            confirmButtonColor: '#00BCD4',
                            cancelButtonColor: '#EF5350',
                            confirmButtonText: 'Aceptar',
                            cancelButtonText: 'Cancelar'
                        }).then(function () {
                            Grabar();
                        });
                    } else if (validacion == 2) {
                        swal({
                            type: 'ERROR',
                            type: 'error',
                            text: 'Existen items con cantidad 0 o menor a 0',
                            confirmButtonColor: '#00BCD4'
                        });
                    } else if (validacion == 3) {
                        swal({
                            type: 'ERROR',
                            type: 'error',
                            text: 'Existen items con stock insuficiente',
                            confirmButtonColor: '#00BCD4'
                        });
                    }
                });
            });

            $("#btnAgregarItem").on('click', function () {
                var selected = $("select[name='sltItem']").find('option:selected');
                var item = {
                    ID_ITEM: selected.val(),
                    CODIGO: selected.data("codigo"),
                    ITEM: selected.data("item"),
                    UNIDAD: selected.data("unidad"),
                    CANTIDAD_SOLICITADA: 0,
                    CANTIDAD_MOVIMIENTO: 1,
                    ID_DETALLE: 0,
                    STOCK_ACTUAL: selected.data("stockactual"),
                    COSTO_MOVIMIENTO: 0
                }

                if ($("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.AJUSTE_DE_BODEGA_POR_EGRESO || $("#ID_TIPO_MOVIMIENTO").val() == EnumTipoMovimiento.REQUISICION_BODEGA) { 
                    //Si son movimientos de egreso se valida que tenga stock
                    if (selected.data("stockactual") > 0) {
                        agregarItem(item);
                    } else {
                        swal({
                            type: 'ERROR',
                            type: 'error',
                            text: 'El item seleccionado no tiene stock',
                            confirmButtonColor: '#00BCD4'
                        });
                    }
                } else { 
                    //Si son movimientos de ingreso
                    agregarItem(item);
                }


            });

            $("#btnEliminarItem").on('click', function () {
                var filas = tabItems.column(6).checkboxes.selected();

                if (filas.length > 0) {
                    var titulo;
                    if (filas.length == 1) { titulo = '¿Seguro desea eliminar el registro seleccionado?'; }
                    else { titulo = '¿Seguro desea eliminar los ' + filas.length + ' registros seleccionados?'; }
                    swal({
                        text: titulo,
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#00BCD4',
                        cancelButtonColor: '#EF5350',
                        confirmButtonText: 'Aceptar',
                        cancelButtonText: 'Cancelar'
                    }).then(function () {
                        tabItems.$("input[type='checkbox']").each(function () {
                            if (this.checked) {
                                var fila = $(this).closest('tr');
                                linea = tabItems.row(tabItems.row(fila).index());
                                tabItems.row(tabItems.row(fila).index()).remove().draw();
                            }
                        });
                    });
                }
            });

            if (!registro_nuevo) {
                $("#ID_TIPO_MOVIMIENTO").select2({ disabled: true });
                $("#OBSERVACION").prop('readonly', true);
                columnstock = tabItems.column(4);
                columnstock.visible(false);
                columcheck = tabItems.column(6);
                columcheck.visible(false);
            }

            $("#btnRelacion").on("click", function () {
                if (tipoMovimientoSeleccionado == 2) {
                    BuscarSolicitudAutorizada();
                } else if (tipoMovimientoSeleccionado == 4) {
                    BuscarOrdenCompra();
                }
            });

            $("#butSeleccionaDocumento").on("click", function () {
                var documentoSeleccionado = tabDocumentoRelacionado.rows('.selected').data()[0];
                if (tipoMovimientoSeleccionado == 2) {
                    $("#lblSolicitudRequisicion").text(documentoSeleccionado.ANIO + " - " + documentoSeleccionado.NUMERO_MOVIMIENTO);
                    $("#lblFechaAutorizacion").text(documentoSeleccionado.FECHA);
                    $("#lblUsuarioSolicita").text(documentoSeleccionado.USUARIO_SOLICITA);
                    $("#lblDptoSolicita").text(documentoSeleccionado.DEPARTAMENTO);
                    DetalleSolicitud(documentoSeleccionado.ID_MOVIMIENTO);
                }
                
                $("#dlgDocumentoRelacionado").modal('toggle');
            });

            $("#butSeleccionaOC").on("click", function () {
                var ocSeleccionado = tabOrdenCompra.rows('.selected').data()[0];
                $("#lblOrdenCompra").text(ocSeleccionado.ANIO + " - " + ocSeleccionado.NUMERO_MOVIMIENTO);
                $("#lblProveedor").text(ocSeleccionado.PROVEEDOR);
                $("#lblFactura").text(ocSeleccionado.FACTURA);
                $("#lblFechaFactura").text(ocSeleccionado.FECHA);
                DetalleOrdenCompra(ocSeleccionado.ANIO, ocSeleccionado.NUMERO_MOVIMIENTO);
                $("#dlgOrdenCompra").modal('toggle');
            });

            $("#btnImprimir").on('click', function () {
                event.preventDefault();
                Imprimir();
            });
        }
    };
}();
var tabItems;
var tipoMovimientoSeleccionado;
var ldata;

function agregarItem(item) {
    if (item.ID_ITEM !== undefined) {
        producto = item.DESCRIPCION;
        var cantidad = `<div class="col-md-1">
                            <input id=txtcant${item.ID_ITEM} name="detalle-cantidad" type="number" style="width: 75px;" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                        </div>`;
        var row = []
        row.push({
            "CODIGO": item.CODIGO,
            "DESCRIPCION": item.ITEM,
            "CANTIDAD_SOLICITADA": 0,
            "CANTIDAD_PEDIDO": cantidad,
            "STOCK_ACTUAL": item.STOCK_ACTUAL,
            "ID_DETALLE": 0,
            "ID_ITEM": item.ID_ITEM
        });
        //var index = tabItems.row.add(row).draw(false).index();
        tabItems.rows.add(row).draw()
        //tabItems.columns.adjust().draw();
        $("#txtcant" + item.ID_ITEM).on('keypress', function (e) {
            if (e.which === 13) {
                var cantidad
                if ($(this).val() == "") {
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
                //var items = data.items;
                //ldata = $.parseJSON(data.data);
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
                //var items = data.items;
                //ldata = $.parseJSON(data.data);
                ldata = data.data;
                tabDocumentoRelacionado.clear();
                tabDocumentoRelacionado.rows.add(data.data);
                tabDocumentoRelacionado.draw();
                $("#titDocumentoRelacionado").text("Ordenes de Compra");
                $("#dlgDocumentoRelacionado").modal('toggle');
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
                    var cantidad = `<div class="col-md-1">
                            <input id=txtcant${i} name="detalle-cantidad" type="number" style="width: 75px;" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                        </div>`;
                    var row = []
                    row.push({
                        "CODIGO": ldata[i].CODIGO,
                        "DESCRIPCION": ldata[i].ITEM,
                        "CANTIDAD_SOLICITADA": ldata[i].CANTIDAD,
                        "CANTIDAD_PEDIDO": cantidad,
                        "STOCK_ACTUAL": ldata[i].STOCK_ACTUAL,
                        "ID_DETALLE": 0,
                        "ID_ITEM": ldata[i].ID_ITEM
                    });
                    tabItems.rows.add(row).draw();
                    tabItems.columns.adjust().draw();
                    column = tabItems.column(5);
                }
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
                    var cantidad = `<div class="col-md-1">
                            <input id=txtcant${i} name="detalle-cantidad" type="number" style="width: 75px;" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                        </div>`;
                    //var row = [];
                    //row.push(ldata[i].CODIGO);
                    //row.push(ldata[i].ITEM);
                    //row.push(ldata[i].CANTIDAD);
                    //row.push(cantidad);
                    //row.push(ldata[i].STOCK_ACTUAL);
                    //row.push(0);
                    //var index = tabItems.row.add(row).draw(false).index();
                    //tabItems.columns.adjust().draw();

                    var row = []
                    row.push({
                        "CODIGO": ldata[i].CODIGO,
                        "DESCRIPCION": ldata[i].ITEM,
                        "CANTIDAD_SOLICITADA": ldata[i].CANTIDAD,
                        "CANTIDAD_PEDIDO": cantidad,
                        "STOCK_ACTUAL": ldata[i].STOCK_ACTUAL,
                        "ID_DETALLE": 0,
                        "ID_ITEM": ldata[i].ID_ITEM
                    });
                    tabItems.rows.add(row).draw();
                    tabItems.columns.adjust().draw();

                    column = tabItems.column(5);
                }
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

function Grabar() {
    var movimiento;
    var movimientoDetalle = [];
    //Validar cabecera
    $('#frmMovimiento').parsley().validate();
    if ($('#frmMovimiento').parsley().isValid()) {
        //Si la cabecera es valida, se valida los detalles
        var validarDetalle = true;
        if (tabItems.rows().count() > 0) {
            $("#tabItems tbody tr").each(function () {
                var row = $(this).closest('tr');

                if ($('input[name="detalle-cantidad"]', row).val() > 0) {
                    movimientoDetalle.push({
                        "ID_DETALLE": 0,
                        "ID_ITEM": row.data("ID_ITEM"),
                        "CANTIDAD_PEDIDO": $('input[name="detalle-cantidad"]', row).val()
                    });
                } else {
                    swal({
                        type: 'ERROR',
                        type: 'error',
                        text: 'Existen items con cantidad 0 o menor a 0',
                        confirmButtonColor: '#00BCD4'
                    });
                    validarDetalle = false;
                    return false;
                }
            });

            if (validarDetalle) {
                movimiento = {
                    "ID_MOVIMIENTO": 0,
                    "ID_TIPO_MOVIMIENTO": $("#ID_TIPO_MOVIMIENTO").val(),
                    "OBSERVACION": $("#OBSERVACION").val(),
                    "EPRTA_MOVIMIENTO_DETALLE": movimientoDetalle
                }
                var parametros = JSON.stringify({
                    pmovimiento: solicitud,
                    pregistro_nuevo: registro_nuevo
                });
                alert('si graba');
            } else {
                alert('no graba');
            }

            //$.ajax({
            //    type: "POST",
            //    traditional: true,
            //    datatype: "json",
            //    data: parametros,
            //    contentType: 'application/json; charset=utf-8',
            //    url: "/Solicitud/Grabar",
            //    beforeSend: function () {
            //        run_waitMe($(".box-primary"), 'Grabando...');
            //    },
            //    success: function (data) {
            //        if (data.resultado == "success") {
            //            swal({
            //                type: 'success',
            //                text: 'Datos grabados',
            //                timer: 20000,
            //                confirmButtonColor: '#00BCD4'
            //            }).then(function () {
            //                window.location.href = '/Solicitud/ListaSolicitud';
            //            });

            //        }
            //        else {
            //            swal({
            //                type: data.resultado,
            //                text: 'Error al grabar los datos. ' + data.mensaje,
            //                confirmButtonColor: '#00BCD4'
            //            });
            //        }
            //        $(".box-primary").waitMe('hide');
            //    },
            //    error: function (XMLHttpRequest, textStatus, errorThrown) {
            //        $(".box-primary").waitMe('hide');
            //        swal({
            //            type: 'error',
            //            text: 'Error al cargar los datos.',
            //            confirmButtonColor: '#00BCD4'
            //        });
            //    },
            //    complete: function () {
            //    }
            //});

        } else {
            swal({
                type: 'ERROR',
                type: 'error',
                text: 'Debe incluir algun item a solicitar',
                confirmButtonColor: '#00BCD4'
            });
        }
    }
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
                    { "targets": 3, "width": 350 },
                    { "targets": 4, "width": 50}
                ],
                "columns": [
                    { "data": 'ID_MOVIMIENTO' },
                    { "data": 'ANIO' },
                    { "data": 'NUMERO_MOVIMIENTO' },
                    { "data": 'REFERENCIA' },
                    { "data": 'FECHA' }
                ]
            });
            
            tabItems= $('#tabItems').DataTable({
                "autoWidth": false,
                "paging": false,
                "columnDefs":
                    [
                        { "targets": [5], "checkboxes": { 'selectRow': false } },
                        { "targets": [6], "visible": false }
                    ],
                "columns": [
                    { "data": 'CODIGO' },
                    { "data": 'DESCRIPCION' },
                    { "data": 'CANTIDAD_SOLICITADA' },
                    { "data": 'CANTIDAD_PEDIDO' },
                    { "data": 'STOCK_ACTUAL' },
                    { "data": 'ID_DETALLE' },
                    { "data": 'ID_ITEM' }
                ]
            });

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
                //minimumInputLength: 1,
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
                //var data = e.params.args.data;
                //console.log(data);
                //agregarItem();
                $("#btnAgregarItem").focus();
            });

            $("#btnCancelar").on('click', function () {
                window.location.href = '/Movimiento/ListaMovimiento';
            });

            $('#ID_TIPO_MOVIMIENTO').on('select2:select', function (e) {
                
                tipoMovimientoSeleccionado = e.params.data.id;
                if (tipoMovimientoSeleccionado == 2 || tipoMovimientoSeleccionado == 4) {
                    $("#divBuscaRelacion").css('display', 'block');
                    $("#divAgregar").css('display', 'none'); 
                } else {
                    $("#divBuscaRelacion").css('display', 'none');
                    $("#divAgregar").css('display', 'block'); 
                }
            });

            $("#btnGrabar").on('click', function () {
                Grabar();
            });


            $("#btnAgregarItem").on('click', function () {
                var selected = $("select[name='sltItem']").find('option:selected');
                var item = {
                    ID_ITEM: selected.val(),
                    CODIGO: selected.data("codigo"),
                    ITEM: selected.data("item"),
                    UNIDAD: selected.data("unidad"),
                    CANTIDAD_PEDIDO: 1,
                    ID_DETALLE: 0,
                    STOCK_ACTUAL: selected.data("stockactual")
                }
                agregarItem(item);
            });

            $("#btnEliminarItem").on('click', function () {
                var filas = tabItems.column(5).checkboxes.selected();

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
                    DetalleSolicitud(documentoSeleccionado.ID_MOVIMIENTO);
                    $("#lblOrdenCompra").text("");
                } else if (tipoMovimientoSeleccionado == 4) {
                    $("#lblOrdenCompra").text(documentoSeleccionado.ANIO + " - " + documentoSeleccionado.NUMERO_MOVIMIENTO);
                    $("#lblSolicitudRequisicion").text("");
                    DetalleOrdenCompra(documentoSeleccionado.ANIO, documentoSeleccionado.NUMERO_MOVIMIENTO);
                }
                
                $("#dlgDocumentoRelacionado").modal('toggle');
            });

            $(document).on('click', '.pdf-autorizado', function (event) {
                event.preventDefault();
                $("#modal-dialog-print-pdf").modal("show");
                var id = $(this).data("id");
                $(".modal-title").text("FACTURA");
                $('#TagEmbed').attr("src", "/FacturaVenta/ViewPDF/" + id);
            });
        }
    };
}();
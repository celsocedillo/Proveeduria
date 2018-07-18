var tabItems;
var tipoMovimientoSeleccionado;
var ldata;

function agregarItem(item) {
    if (item.ID_ITEM !== undefined) {
        producto = item.DESCRIPCION;
        var cantidad = `<div class="col-md-1">
                            <input id=txtcant${item.ID_ITEM} name="detalle-cantidad" type="number" style="width: 75px;" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                        </div>`;
        var row = [];
        row.push(item.CODIGO);
        row.push(item.ITEM);
        row.push(0);
        row.push(cantidad);
        row.push(item.STOCK_ACTUAL);
        var index = tabItems.row.add(row).draw(false).index();
        tabItems.columns.adjust().draw();
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
                    var row = [];
                    row.push(ldata[i].CODIGO);
                    row.push(ldata[i].ITEM);
                    row.push(ldata[i].CANTIDAD);
                    row.push(cantidad);
                    row.push(ldata[i].STOCK_ACTUAL);
                    var index = tabItems.row.add(row).draw(false).index();
                    tabItems.columns.adjust().draw();
                }
                //tabItems.clear();
                //tabItems.rows.add(data.data);
                //tabItems.draw();
                /*$("#titDocumentoRelacionado").text("Solicitudes de requisición de bodega");
                $("#dlgDocumentoRelacionado").modal('toggle');*/
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
                "paging": false
                //"bSortable": false
                //"columnDefs":
                //    [
                //        { "targets": [0, 1, 2, 3], "orderable": false, "className": "text-right" }
                //    ]
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
                } else {
                    $("#divBuscaRelacion").css('display', 'none');
                }
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
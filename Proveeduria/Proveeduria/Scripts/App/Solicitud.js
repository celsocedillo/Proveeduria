var tabItems

function agregarItem(item) {
    if (item.ID_ITEM !== undefined) {
        producto = item.DESCRIPCION;
        var cantidad = `<div class="col-md-1">
                            <input id=txtcant${item.ID_ITEM} name="detalle-cantidad" type="number" style="width: 75px;" class="form-control text-right txtcant no-spin" value="${item.CANTIDAD_PEDIDO}" min=0 step="any" required>
                        </div>`;
        //var eliminar = `<a title="Eliminar" class="remove">
        //                    <span class="fa-stack fa-1x text-inverse">
        //                        <i class="fa fa-circle-thin fa-stack-2x"></i>
        //                        <i class="fa fa-trash fa-stack-1x"></i>
        //                    </span>
        //                </a>`;
        var eliminar = `<input type="checkbox" class="dt-checkboxes">`
        var row = [];
        row.push(item.CODIGO);
        row.push(item.DESCRIPCION);
        row.push(cantidad);
        row.push(item.UNIDAD);
        row.push(item.ID_DETALLE);
        var index = tabItems.row.add(row).draw(false).index();
        tabItems.rows(index).nodes().to$().attr("data-id_item", item.ID_ITEM);
        tabItems.rows(index).nodes().to$().attr("data-id_detalle", item.ID_DETALLE);
        tabItems.rows(index).nodes().to$().attr("data-estado", item.ESTADO);
        tabItems.columns.adjust().draw();

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

        $("#txtcant" + item.ID_ITEM).focusout(function () {
            if ($(this).val() == "") {
                $(this).val("0");
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

function Grabar() {
    var solicitud;
    var solicitudDetalle = [];
    //Validar cabecera
    $('#frmSolicitud').parsley().validate();
    if ($('#frmSolicitud').parsley().isValid()) {
        //Si la cabecera es valida, se valida los detalles
        if (tabItems.rows().count() > 0) {
            $("#tabItems tbody tr").each(function () {
                var row = $(this).closest('tr');
                solicitudDetalle.push({
                    "ID_DETALLE": row.data("id_detalle"),
                    "ID_ITEM": row.data("id_item"),
                    "CANTIDAD_PEDIDO": $('input[name="detalle-cantidad"]', row).val(),
                    "ESTADO": row.data("estado")
                });
            });

            //if ($('#ESTADO').val() == "A")
            //{
            //    estadoSolicitud = $('#ESTADO').val();
            //}

            
            solicitud = {
                "ID_MOVIMIENTO": solicitudId,
                "OBSERVACION": $("#OBSERVACION").val(),
                "ESTADO": $('#ESTADO').val(),
                "EPRTA_MOVIMIENTO_DETALLE": solicitudDetalle
            }
            var parametros = JSON.stringify({
                pmovimiento: solicitud,
                pregistro_nuevo: registro_nuevo
            });
            $.ajax({
                type: "POST",
                traditional: true,
                datatype: "json",
                data: parametros,
                contentType: 'application/json; charset=utf-8',
                url: "/Solicitud/Grabar",
                beforeSend: function () {
                    run_waitMe($(".box-primary"), 'Grabando...');
                },
                success: function (data) {
                    if (data.resultado == "success") {
                        swal({
                            type: 'success',
                            text: 'Datos grabados',
                            timer: 20000,
                            confirmButtonColor: '#00BCD4'
                        }).then(function () {
                            window.location.href = '/Solicitud/ListaSolicitud';
                        });

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

var Solicitud = function () {
    return {
        init: function () {

            tabItems = $('#tabItems').DataTable({
                "title": "Items por Bodega",
                "paging": false,
                "autoWidth": false,
                "filter": false,
                "info": false,
                "columnDefs":
                    [
                        { "targets": [2], "className": "text-right" },
                        { "targets": [4], "checkboxes": { 'selectRow': false } },
                    ],
                "select": {
                    "style" : "multi"
                }
            });

            if (estadoSolicitud == "S" || estadoSolicitud == "A") {
                for (i in detalleMovimiento) {
                    agregarItem(detalleMovimiento[i]);
                }
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
                //minimumInputLength: 1,
                templateResult: function (repo) {
                    return "("+repo.CODIGO + ") " + repo.DESCRIPCION;
                },
                templateSelection: function (container) {
                    $(container.element).attr("data-codigo", container.CODIGO);
                    $(container.element).attr("data-descripcion", container.DESCRIPCION);
                    $(container.element).attr("data-unidad", container.MEDIDA);
                    $(container.element).attr("data-idItem", container.id);
                    return "(" + container.CODIGO + ") " + container.DESCRIPCION;
                }
            });

            $('#sltItem').on('select2:select', function (e) {
                //var data = e.params.args.data;
                //console.log(data);
                //agregarItem();
                $("#btnAgregarItem").focus();
            });

            $("#btnCancelar").on('click', function () {
                window.location.href = '/Solicitud/ListaSolicitud';
            });

            $("#btnAgregarItem").on('click', function () {
                var selected = $("select[name='sltItem']").find('option:selected');
                var item = {
                    ID_ITEM: selected.val(),
                    CODIGO: selected.data("codigo"),
                    DESCRIPCION: selected.data("descripcion"),
                    UNIDAD: selected.data("unidad"),
                    CANTIDAD_PEDIDO: 1,
                    ID_DETALLE : 0
                }
                agregarItem(item);
            });

            $("#btnGrabar").on('click', function () {
                Grabar();
            });

            $("#btnEliminarItem").on('click', function () {
                var filas = tabItems.column(4).checkboxes.selected();

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
                                fila.attr('data-estado', 'E');
                                fila.css('display', 'none');
                            }
                        });
                    });

                }
            });

            $("#btnImprimir").on('click', function () {
                event.preventDefault();
                $("#modal-dialog-print-pdf").modal("show");
                //var id = $(this).data("id");
                $(".modal-title").text("SOLICITUD");
                $('#TagEmbed').attr("src", "/Solicitud/ViewPDF/" + solicitudId);

            });

          
        }
    };
}();
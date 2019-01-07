var tablaPuntoReOrden;
var fechaInicio;
var fechaFin;

function filtroNuevo() {
    localStorage.removeItem('Filtro');
    nombre = $("#nombre").val();
    activo = $("#EstadoID").val();
    destacado = $("#DestacadoID").val();
    categoria = parseInt($("#CategoriaID").val());
    inicio = $("#inicio").val();
    fin = $("#fin").val();
    if (nombre == "") { nombre = "vacio" }
    if (inicio == "") { inicio = "vacio" }
    if (fin == "") { fin = "vacio" }
    localStorage['Filtro'] = '{ "nombre": "' + nombre + '","activo": "' + activo + '","destacado": "' + destacado + '","categoria": "' + categoria + '","inicio": "' + inicio + '","fin": "' + fin + '" }';
    tablaLocales.ajax.reload();
}

function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/GetPuntoReOrden",
        beforeSend: function () {
            run_waitMe($(".box"), 'Cargando...');
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
                tablaPuntoReOrden.clear();
                tablaPuntoReOrden.rows.add(data.data);
                tablaPuntoReOrden.draw();
            }
            $(".box").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box").waitMe('hide');
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


var Consulta = function () {
    return {
        init: function () {

            $.fn.dataTable.moment('DD/MM/YYYY');

            $("#txtFechaInicio").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            $("#txtFechaFin").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            fechaInicio = $("#FechaInicio").val();
            fechaFin = $("#FechaFin").val();
            if (fechaInicio == "" || fechaInicio == undefined) {
                fechaInicio = "vacio"
            }
            if (fechaFin == "" || fechaFin == undefined) {
                fechaFin = "vacio"
            }

            //tablaPuntoReOrden = $("#tablaPuntoReOrden").DataTable({
            //    autoWidth: false,
            //    processing: true,
            //    scrollX: true,
            //    serverSide: true,
            //    info: true,
            //    stateSave: true,
            //    ajax: {
            //        url: "/Consulta/GetPuntosReOrden",
            //        type: "POST",
            //        data: function (d) {
            //            d.inicio = fechaInicio,
            //            d.fin = fechaFin
            //        },
            //        error: function () {
            //        }
            //    },
            //    columns: [
            //        { data: "item", orderable: false },
            //        { data: "maximo", orderable: true },
            //        { data: "minimo", orderable: false },
            //        { data: "critica", orderable: true },
            //        { data: "inicio", orderable: true },
            //        { data: "actual", orderable: false },
            //        { data: "usado", orderable: false },
            //        { data: "messiete", orderable: false }
            //    ],
            //    "order": [[0, 'desc']]
            //});

            tablaPuntoReOrden = $("#tablaPuntoReOrden").DataTable({
                "autoWidth": false,
                "pageLength": 25,
                "columns": [
                    { "data": "CODIGO", orderable: true },
                    { "data": "ITEM", orderable: true },
                    { "data": "CANTIDAD_MAXIMA", orderable: true },
                    { "data": "CANTIDAD_MINIMA", orderable: false },
                    { "data": "CANTIDAD_CRITICA", orderable: false },
                    { "data": "CANTIDAD_INICIO", orderable: false },
                    { "data": "CANTIDAD_ACTUAL", orderable: false },
                    { "data": "USADO", orderable: false },
                    { "data": "MES_ANTERIOR", orderable: false }
                ],
                "order": [[0, 'desc']]
            });


            $("#btnConsult").click(function (event) {
                //event.preventDefault();
                //fechaInicio = $("#FechaInicio").val();
                //fechaFin = $("#FechaFin").val();
                //if (fechaInicio == "" || fechaInicio == undefined) {
                //    fechaInicio = "vacio"
                //}
                //if (fechaFin == "" || fechaFin == undefined) {
                //    fechaFin = "vacio"
                //}
                //tablaPuntoReOrden.ajax.reload();

                if (ValidarFecha($("#txtFechaInicio").val()) && ValidarFecha($("#txtFechaFin").val())) {
                    parametros = JSON.stringify(
                        {
                            pFechaInicio: $("#txtFechaInicio").val(),
                            pFechaFin: $("#txtFechaFin").val()
                        });
                    CargaDatos();
                } else {
                    swal({
                        type: 'error',
                        text: 'Fechas invalidas, revise el formato de las fechas',
                        confirmButtonColor: '#00BCD4'
                    });

                }


            });

            $("#btnExcel").click(function (event) {
                event.preventDefault();
                var fechaInicio = $("#FechaInicio").val();
                var fechaFin = $("#FechaFin").val();
                if (fechaInicio == "" || fechaInicio == undefined) {
                    fechaInicio = "vacio"
                }
                if (fechaFin == "" || fechaFin == undefined) {
                    fechaFin = "vacio"
                }
                window.location.href = '/Consulta/ExportToExcelPuntosReOrden?fechaInicio=' + fechaInicio + '&fechaFin=' + fechaFin;
            });

        }
    };
}();

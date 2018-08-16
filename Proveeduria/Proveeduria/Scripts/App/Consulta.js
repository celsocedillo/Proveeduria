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


var Consulta = function () {
    return {
        init: function () {

            inicio = $("#inicio").val();
            fin = $("#fin").val();
            if (inicio == "") {
                inicio = "vacio"
            }
            if (fin == "") {
                fin = "vacio"
            }

            tablaPuntoReOrden = $("#tablaPuntoReOrden").DataTable({
                autoWidth: false,
                processing: true,
                scrollX: true,
                serverSide: true,
                info: true,
                stateSave: true,
                ajax: {
                    url: "/Consulta/GetPuntosReOrden",
                    type: "POST",
                    data: function (d) {
                        d.inicio = fechaInicio,
                        d.fin = fechaFin
                    },
                    error: function () {
                    }
                },
                columns: [
                    { data: "item", orderable: false },
                    { data: "maximo", orderable: true },
                    { data: "minimo", orderable: false },
                    { data: "critica", orderable: true },
                    { data: "inicio", orderable: true },
                    { data: "actual", orderable: false },
                    { data: "usado", orderable: false },
                    { data: "messiete", orderable: false }
                ],
                "order": [[0, 'desc']]
            });


            $("#btnConsult").click(function (event) {
                event.preventDefault();
                inicio = $("#inicio").val();
                fin = $("#fin").val();
                if (inicio == "") {
                    inicio = "vacio"
                }
                if (fin == "") {
                    fin = "vacio"
                }
                tablaPuntoReOrden.ajax.reload();
            });
     
        }
    };
}();

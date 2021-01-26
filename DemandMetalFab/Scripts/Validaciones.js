var ban = true;

function validacion(valor, campo) {
    if (isNaN(valor)) {
        //ban = false;
        //return alert('The ' + campo + ' field is not numeric ');
        error('The ' + campo + ' field is not numeric ');
        return false;
    } else {
                if (validarvacio(valor, campo)) {
                    return true;
                } else {
                    return false;
                }
           }

}

function validarvacio(valor, campo) {
    if (valor == null || valor == "") {
        //ban = false;
        //return alert('The ' + campo + ' parts field cannot be empty');
        error('The ' + campo + ' parts field cannot be empty');
        return false;
    }
    return true;
}

function validarNumEmpity(valor) {
    if (isNaN(valor)) {
        error('The field value is not numeric');
        return false;
    }
    if (!validarEmpity(valor)) {
        return false;
    }
    return true;
}

function validarEmpity(valor) {
    if (valor == null || valor == "") {
        
        error('The field cannot be empty');
        return false;
    }
    return true;
}

function error(mensaje) {
    bootoast.toast({
        message: mensaje,
        type: 'danger',
        position: 'bottom-center',
        timeout: null
    });
}
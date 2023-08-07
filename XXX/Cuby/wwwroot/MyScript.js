//Скрипт при перемотки при первой загрузки чата
function loadMessageScroll() {
    var block = document.getElementById("scroll");

    block.scrollTo({
        top: 9999,  
        behavior: 'auto'
    });

}
//после выполнения 1 функции  интерпритатор перезаписал на вторую и следовательно теперь она функционирует
//перемотка при последующих сообщениях
function lastMessageScroll() {
    var block = document.getElementById("scroll");
        
    block.scrollTo({
        top: 9999,
        behavior: 'smooth'
    });
   
}
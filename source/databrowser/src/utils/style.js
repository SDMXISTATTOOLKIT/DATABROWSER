const $ = window.jQuery;

/* https://stackoverflow.com/questions/986937/how-can-i-get-the-browsers-scrollbar-sizes */
export function getScrollBarWidth() {
  const temp = $('<div>').css({visibility: 'hidden', width: 100, overflow: 'scroll'}).appendTo('body');
  const widthWithScroll = $('<div>').css({width: '100%'}).appendTo(temp).outerWidth();
  temp.remove();
  return 100 - widthWithScroll;
}

export function getTextWidth(text, element) {
  if (typeof (text) === "string" && text.length > 0) {
    element.innerText = text;
    return element.offsetWidth;
  } else {
    return 0;
  }
}

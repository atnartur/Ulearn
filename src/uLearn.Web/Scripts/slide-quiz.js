﻿function submitQuiz(courseId, slideIndex, expectedCount, isLti) {
	if (areAllAnswered(expectedCount)) {
		var answers = [];
		$(".quiz").each(function () {
			var id = $(this).find('input').attr('id'); //id of quiz
			var content = id.split('quizBlock');
			var val;
			if ($(this).hasClass('quiz-block-input')) {
				val = $(this).find('input').val();
				answers.push(new QuizAnswer("Text", content[0], "", val));
			}
			if ($(this).hasClass('quiz-block-ordering__item')) {
				val = $(this).data('item-id');
				answers.push(new QuizAnswer("Ordering", content[0], "", val));
			}
			if ($(this).hasClass('quiz-block-matching__item')) {
				var orderId = $(this).find('.quiz-block-matching__droppable').data('item-id');
				val = $(this).find('.quiz-block-matching__droppable').data('movable-item-id');
				answers.push(new QuizAnswer("Matching", content[0], orderId, val));
			}
			if ($('#' + id).is(':checked')) {
				var type = content[1] == "True" || content[1] == "False" ? "TrueFalse" : "Checkbox";
				answers.push(new QuizAnswer(type, content[0], content[1], ""));
			}
		});
		var answer = JSON.stringify(answers);
		$.ajax(
			{
				type: "POST",
				url: $("#SubmitQuiz").data("url"),
				data: {
					courseId: courseId,
					slideIndex: slideIndex,
					answer: answer,
					isLti: isLti
				}
			}).success(function (ans) {
				$("#quiz-status").text("Проверяется...");
				window.scrollTo(0, 0);
				window.location.reload();
			})
			.fail(function (req) {
				console.log(req.responseText);
			})
			.always(function (ans) {
			});
		console.log(answers);
		return answer;
	} else
		alert("Fill this quiz!");
};


function areAllAnswered(needingCount) {
	var realCount = 0;
	$(".quiz-block-mark").each(function () {
		if ($(this).children().children().is(':checked')) {
			realCount++;
		}
	});
	$(".quiz-block-input").each(function () {
		if ($(this).find('input').val() != "") {
			realCount++;
		}
	});
	realCount += $('.quiz-block-ordering').length;
	$(".quiz-block-matching").each(function () {
		var unfilled = $.grep($(this).find('.quiz-block-matching__droppable'), function(el) {
			return $(el).data('movable-item-id') === undefined;
		});
		if (unfilled.length === 0)
			realCount++;
		
	});
	return realCount >= needingCount;
}


function QuizAnswer(type, quizId, itemId, text) {
	this.QuizType = type;
	this.QuizId = quizId;
	this.ItemId = itemId;
	this.Text = text;
}

$(document).ready(function() {
	$.fn.moveUp = function() {
		$.each(this, function() {
			$(this).after($(this).prev());
		});
	};

	$.fn.moveDown = function() {
		$.each(this, function() {
			$(this).before($(this).next());
		});
	};

	$(".quiz-block-ordering ul").sortable();

	$(".quiz-block-ordering .quiz-block-ordering__item .quiz-block-ordering__item__icons").click(function(e) {
		var $target = $(e.target);
		var item = $target.closest("li");
		if ($target.is(".glyphicon-arrow-down"))
			item.moveDown();
		if ($target.is(".glyphicon-arrow-up"))
			item.moveUp();
	});


	$('.quiz-block-matching:not(.not-movable) .quiz-block-matching__movable-item').draggable({
		revert: "invalid",
	});

	$('.quiz-block-matching:not(.not-movable) .quiz-block-matching__droppable')
		.add('.quiz-block-matching:not(.not-movable) .quiz-block-matching__source__droppable').each(function () {
		var $this = $(this);
		var blockId = $(this).data('block-id');
		$this.droppable({
			activeClass: 'active',
			hoverClass: 'hover',
			accept: '[data-block-id=' + blockId + ']',
			drop: function (event, ui) {
				if ($(this).hasClass('added'))
					return;
				$(this).addClass('added');
				var $dropped = $(ui.draggable);
				var $droppedOn = $(this);

				$dropped.offset($droppedOn.offset());
				$droppedOn.data('movable-item-id', $dropped.data('item-id'));
			},
			out: function (event, ui) {
				var $dropped = $(ui.draggable);
				if ($(this).data('movable-item-id') === $dropped.data('item-id')) {
					$(this).removeClass('added');
					$(this).removeData('movable-item-id');
				}
			}
		});
	});

	$('.quiz-block-ordering__correct-button').on('mousedown mouseup', function() {
		var $block = $(this).closest('.quiz-block-ordering');
		var $items = $block.find('.quiz-block-ordering__item');
		var order = $(this).data('order').split(',');
		$(this).data('order', $items.map(function () { return $(this).data('item-id'); }).get().join(','));
		order.forEach(function(itemId, idx) {
			var $item = $items.filter('[data-item-id=' + itemId + ']');
			$item.appendTo($block.find('li').get(idx));
		});
	});

	$('.quiz-block-matching__correct-button').on('mousedown mouseup', function () {
		var $block = $(this).closest('.quiz-block-matching');
		var $items = $block.find('.quiz-block-matching__movable-item');
		var order = $(this).data('order').split(',');
		$(this).data('order', $items.map(function () { return $(this).data('item-id'); }).get().join(','));
		order.forEach(function (itemId, idx) {
			var $item = $items.filter('[data-item-id=' + itemId + ']');
			$item.appendTo($block.find('.quiz-block-matching__droppable').get(idx));
		});
	});
});
<script setup lang="ts">
    var props = defineProps<{
        value: boolean
    }>();
</script>
<template>
    <icon v-if="value" name="check" class="text-success"/>
    <icon v-else-if="value == false" name="xmark" class="text-danger"/>
    <icon v-else name="question" class="text-warning"/>
</template>